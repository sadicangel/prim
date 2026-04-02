using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using CodeAnalysis.Binding;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation.Values;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Declarations;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Evaluation;

internal sealed class Interpreter : IBoundNodeVisitor<PrimValue>
{
    private readonly Dictionary<Symbol, PrimValue> _evaluations = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<BoundNode, BoundTreeIndex> _indexes = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<Symbol> _evaluating = new(ReferenceEqualityComparer.Instance);
    private readonly Stack<BoundTreeIndex> _activeIndexes = [];
    private readonly Stack<EvaluationFrame> _frames = [];

    public PrimValue Interpret(BoundNode node) => Evaluate(node);

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundModuleDeclaration node)
    {
        return CacheValue(node.Module, node, () =>
        {
            var moduleValue = new ModuleValue(node.Module, node.Module.IsGlobal ? null! : ResolveModuleValue(node.Module.ContainingModule));
            _evaluations[node.Module] = moduleValue;

            foreach (var member in node.Members)
            {
                var memberValue = this.Visit(member);
                if (TryGetDeclaredSymbol(member, out var symbol))
                {
                    moduleValue.Set(symbol, memberValue);
                }
            }

            return moduleValue;
        });
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundPredefinedDeclaration node)
    {
        return CacheValue(node.Symbol, node, () => node.Symbol switch
        {
            StructTypeSymbol structType => new StructValue(structType, structType.ContainingModule.RuntimeType),
            VariableSymbol { Type: LambdaTypeSymbol lambdaType } variable => new LambdaValue(lambdaType, CreateBuiltinDelegate(variable, lambdaType)),
            ModuleSymbol module => new ModuleValue(module, module.IsGlobal ? null! : ResolveModuleValue(module.ContainingModule)),
            _ => throw new NotSupportedException($"Unsupported predefined symbol '{node.Symbol}'.")
        });
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundPropertyDeclaration node)
    {
        return CacheValue(node.Property, node, () =>
            node.Initializer is not null
                ? this.Visit(node.Initializer)
                : CreateDefaultValue(node.Property.Type));
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundStructDeclaration node)
    {
        return CacheValue(node.StructSymbol, node, () =>
        {
            var structValue = new StructValue(node.StructSymbol, node.StructSymbol.ContainingModule.RuntimeType);
            _evaluations[node.StructSymbol] = structValue;

            foreach (var property in node.Properties)
            {
                var propertyValue = this.Visit(property);
                structValue.Set(property.Property, propertyValue);
            }

            return structValue;
        });
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundVariableDeclaration node)
    {
        return CacheValue(node.VariableSymbol, node, () => this.Visit(node.Expression));
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundNopExpression node)
    {
        return CreateValue(node.Type.ContainingModule.Unit, Unit.Value);
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundBinaryExpression node)
    {
        var left = this.Visit(node.Left);
        var right = this.Visit(node.Right);
        var @operator = this.Visit(node.Operator);

        return ((LambdaValue)@operator).Invoke(left, right);
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundBlockExpression node)
    {
        var frame = new EvaluationFrame(node);
        _frames.Push(frame);

        try
        {
            var result = node.Type.MapsToNever
                ? CreateValue(node.Type.ContainingModule.Unit, Unit.Value)
                : CreateDefaultValue(node.Type);
            foreach (var child in node.Expressions)
            {
                result = this.Visit(child);
            }

            return result;
        }
        finally
        {
            _frames.Pop();
        }
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundAssignmentExpression node)
    {
        var value = this.Visit(node.Right);
        AssignValue(node.Left.Symbol, value);
        return value;
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundLambdaExpression node)
    {
        var captured = CaptureVisibleValues();
        return new LambdaValue(node.LambdaType, CreateLambdaDelegate(node, captured));
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundLiteralExpression node)
    {
        return CreateValue(node.Type, node.Value);
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundReference node)
    {
        return ResolveSymbolValue(node.Symbol);
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundNeverExpression node)
    {
        throw new InvalidOperationException($"Cannot interpret '{node.Syntax.SyntaxKind}' because binding produced a never expression.");
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundUnaryExpression node)
    {
        var operand = this.Visit(node.Operand);
        var @operator = this.Visit(node.Operator);

        return ((LambdaValue)@operator).Invoke(operand);
    }

    PrimValue IBoundNodeVisitor<PrimValue>.Visit(BoundInvocationExpression node)
    {
        var callee = this.Visit(node.Callee) as LambdaValue
            ?? throw new InvalidOperationException($"Expected invocation callee '{node.Callee.Type.Name}' to evaluate to a lambda value.");

        var arguments = new PrimValue[node.Arguments.Length];
        for (var i = 0; i < arguments.Length; i++)
        {
            arguments[i] = this.Visit(node.Arguments[i]);
        }

        return callee.Invoke(arguments);
    }

    private PrimValue Evaluate(BoundNode node)
    {
        var index = GetOrCreateIndex(node.GetRoot());
        _activeIndexes.Push(index);

        var pushedFrames = PushLexicalFrames(node);
        try
        {
            return this.Visit(node);
        }
        finally
        {
            PopFrames(pushedFrames);
            _activeIndexes.Pop();
        }
    }

    private int PushLexicalFrames(BoundNode node)
    {
        var pushed = 0;
        foreach (var ancestor in node.EnumerateAncestors().Reverse())
        {
            if (ancestor is not BoundBlockExpression && ancestor is not BoundLambdaExpression)
            {
                continue;
            }

            if (TryGetFrame(ancestor, out _))
            {
                continue;
            }

            _frames.Push(new EvaluationFrame(ancestor));
            pushed++;
        }

        return pushed;
    }

    private void PopFrames(int count)
    {
        while (count-- > 0)
        {
            _frames.Pop();
        }
    }

    private PrimValue ResolveSymbolValue(Symbol symbol)
    {
        if (TryGetValue(symbol, out var value))
        {
            return value;
        }

        if (CurrentIndex?.Declarations.TryGetValue(symbol, out var declaration) is true)
        {
            return this.Visit(declaration);
        }

        if (CurrentIndex?.Parameters.ContainsKey(symbol) is true)
        {
            throw new InvalidOperationException($"Cannot interpret parameter '{symbol.Name}' outside of a lambda invocation.");
        }

        var (boundNode, diagnostics) = symbol.Bind();
        if (diagnostics.HasErrorDiagnostics)
        {
            throw new InvalidOperationException($"Failed to bind '{symbol.FullName}' during interpretation:{Environment.NewLine}{string.Join(Environment.NewLine, diagnostics)}");
        }

        return Evaluate(boundNode.LinkParents());
    }

    private PrimValue CacheValue(Symbol symbol, BoundNode declaration, Func<PrimValue> factory)
    {
        if (TryGetValue(symbol, out var existing))
        {
            return existing;
        }

        if (!_evaluating.Add(symbol))
        {
            throw new InvalidOperationException($"Circular evaluation detected for '{symbol.FullName}'.");
        }

        try
        {
            var value = factory();
            StoreValue(symbol, declaration, value);
            return value;
        }
        finally
        {
            _evaluating.Remove(symbol);
        }
    }

    private void StoreValue(Symbol symbol, BoundNode declaration, PrimValue value)
    {
        if (TryGetOwningFrame(symbol, declaration, out var frame))
        {
            frame.Values[symbol] = value;
            return;
        }

        _evaluations[symbol] = value;
    }

    private void AssignValue(Symbol symbol, PrimValue value)
    {
        if (TryGetAssignmentFrame(symbol, out var frame))
        {
            frame.Values[symbol] = value;
            return;
        }

        _evaluations[symbol] = value;
    }

    private bool TryGetAssignmentFrame(Symbol symbol, out EvaluationFrame frame)
    {
        foreach (var current in _frames)
        {
            if (current.Values.ContainsKey(symbol))
            {
                frame = current;
                return true;
            }
        }

        if (CurrentIndex?.Declarations.TryGetValue(symbol, out var declaration) is true &&
            TryGetOwningFrame(symbol, declaration, out frame))
        {
            return true;
        }

        frame = null!;
        return false;
    }

    private bool TryGetValue(Symbol symbol, [NotNullWhen(true)] out PrimValue? value)
    {
        value = null!;
        foreach (var frame in _frames)
        {
            if (frame.Values.TryGetValue(symbol, out value))
            {
                return true;
            }
        }

        return _evaluations.TryGetValue(symbol, out value!);
    }

    private bool TryGetOwningFrame(Symbol symbol, BoundNode declaration, out EvaluationFrame frame)
    {
        if (CurrentIndex?.Parameters.TryGetValue(symbol, out var parameter) is true)
        {
            return TryGetFrame(parameter.Lambda, out frame);
        }

        if (declaration is BoundVariableDeclaration { Parent: BoundBlockExpression block })
        {
            return TryGetFrame(block, out frame);
        }

        frame = null!;
        return false;
    }

    private bool TryGetFrame(BoundNode owner, out EvaluationFrame frame)
    {
        foreach (var current in _frames)
        {
            if (ReferenceEquals(current.Owner, owner))
            {
                frame = current;
                return true;
            }
        }

        frame = null!;
        return false;
    }

    private Delegate CreateLambdaDelegate(BoundLambdaExpression node, IReadOnlyDictionary<Symbol, PrimValue> captured)
    {
        return node.Parameters.Length switch
        {
            0 => (Func<PrimValue>)(() => InvokeLambda(node, captured, [])),
            1 => (Func<PrimValue, PrimValue>)(arg0 => InvokeLambda(node, captured, [arg0])),
            2 => (Func<PrimValue, PrimValue, PrimValue>)((arg0, arg1) => InvokeLambda(node, captured, [arg0, arg1])),
            3 => (Func<PrimValue, PrimValue, PrimValue, PrimValue>)((arg0, arg1, arg2) => InvokeLambda(node, captured, [arg0, arg1, arg2])),
            4 => (Func<PrimValue, PrimValue, PrimValue, PrimValue, PrimValue>)((arg0, arg1, arg2, arg3) => InvokeLambda(node, captured, [arg0, arg1, arg2, arg3])),
            5 => (Func<PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue>)((arg0, arg1, arg2, arg3, arg4) => InvokeLambda(node, captured, [arg0, arg1, arg2, arg3, arg4])),
            6 => (Func<PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue>)((arg0, arg1, arg2, arg3, arg4, arg5) => InvokeLambda(node, captured, [arg0, arg1, arg2, arg3, arg4, arg5])),
            7 => (Func<PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue>)((arg0, arg1, arg2, arg3, arg4, arg5, arg6) => InvokeLambda(node, captured, [arg0, arg1, arg2, arg3, arg4, arg5, arg6])),
            8 => (Func<PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue, PrimValue>)((arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) => InvokeLambda(node, captured, [arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7])),
            _ => throw new NotSupportedException("The interpreter currently supports lambdas with up to 8 parameters.")
        };
    }

    private PrimValue InvokeLambda(BoundLambdaExpression node, IReadOnlyDictionary<Symbol, PrimValue> captured, ReadOnlySpan<PrimValue> arguments)
    {
        if (arguments.Length != node.Parameters.Length)
        {
            throw new InvalidOperationException($"Lambda expects {node.Parameters.Length} arguments but received {arguments.Length}.");
        }

        var frame = new EvaluationFrame(node, captured);
        for (var i = 0; i < node.Parameters.Length; i++)
        {
            frame.Values[node.Parameters[i]] = arguments[i];
        }

        _frames.Push(frame);
        try
        {
            return Evaluate(node.Body);
        }
        finally
        {
            _frames.Pop();
        }
    }

    private Delegate CreateBuiltinDelegate(VariableSymbol symbol, LambdaTypeSymbol lambdaType)
    {
        return lambdaType.Parameters.Length switch
        {
            1 => (Func<PrimValue, PrimValue>)(arg0 => EvaluateBuiltin(symbol, lambdaType, [arg0])),
            2 => (Func<PrimValue, PrimValue, PrimValue>)((arg0, arg1) => EvaluateBuiltin(symbol, lambdaType, [arg0, arg1])),
            _ => throw new NotSupportedException($"Unsupported builtin arity {lambdaType.Parameters.Length} for '{symbol.Name}'.")
        };
    }

    private PrimValue EvaluateBuiltin(VariableSymbol symbol, LambdaTypeSymbol lambdaType, ReadOnlySpan<PrimValue> arguments)
    {
        return arguments.Length switch
        {
            1 => EvaluateUnaryBuiltin(symbol.Syntax.SyntaxKind, lambdaType.ReturnType, arguments[0]),
            2 => EvaluateBinaryBuiltin(symbol.Syntax.SyntaxKind, lambdaType.Parameters[0], lambdaType.Parameters[1], lambdaType.ReturnType, arguments[0], arguments[1]),
            _ => throw new NotSupportedException($"Unsupported builtin arity {arguments.Length} for '{symbol.Name}'.")
        };
    }

    private PrimValue EvaluateUnaryBuiltin(SyntaxKind kind, TypeSymbol returnType, PrimValue operand)
    {
        var operandType = (StructTypeSymbol)operand.Type;
        return operandType.Name switch
        {
            "bool" => kind switch
            {
                SyntaxKind.BangToken => CreateValue(returnType, !(bool)operand.Value),
                _ => throw new NotSupportedException($"Unsupported unary operator '{kind}' for bool.")
            },
            "i8" => EvaluateNumericUnary<sbyte>(kind, returnType, operand),
            "i16" => EvaluateNumericUnary<short>(kind, returnType, operand),
            "i32" => EvaluateNumericUnary<int>(kind, returnType, operand),
            "i64" => EvaluateNumericUnary<long>(kind, returnType, operand),
            "isz" => EvaluateNumericUnary<nint>(kind, returnType, operand),
            "u8" => EvaluateUnsignedUnary<byte>(kind, returnType, operand),
            "u16" => EvaluateUnsignedUnary<ushort>(kind, returnType, operand),
            "u32" => EvaluateUnsignedUnary<uint>(kind, returnType, operand),
            "u64" => EvaluateUnsignedUnary<ulong>(kind, returnType, operand),
            "usz" => EvaluateUnsignedUnary<nuint>(kind, returnType, operand),
            "f16" => EvaluateNumericUnary<Half>(kind, returnType, operand),
            "f32" => EvaluateNumericUnary<float>(kind, returnType, operand),
            "f64" => EvaluateNumericUnary<double>(kind, returnType, operand),
            _ => throw new NotSupportedException($"Unsupported unary operator '{kind}' for {operandType.Name}.")
        };
    }

    private PrimValue EvaluateBinaryBuiltin(
        SyntaxKind kind,
        TypeSymbol leftType,
        TypeSymbol rightType,
        TypeSymbol returnType,
        PrimValue left,
        PrimValue right)
    {
        if (kind is SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken)
        {
            var equals = Equals(left.Value, right.Value);
            return CreateValue(returnType, kind is SyntaxKind.EqualsEqualsToken ? equals : !equals);
        }

        if (leftType.MapsToAny || rightType.MapsToAny)
        {
            return kind switch
            {
                SyntaxKind.PlusToken => CreateValue(returnType, $"{left.Value}{right.Value}"),
                _ => throw new NotSupportedException($"Unsupported binary operator '{kind}' for any.")
            };
        }

        var concreteLeftType = (StructTypeSymbol)leftType;
        return concreteLeftType.Name switch
        {
            "str" => kind switch
            {
                SyntaxKind.PlusToken => CreateValue(returnType, $"{left.Value}{right.Value}"),
                _ => throw new NotSupportedException($"Unsupported binary operator '{kind}' for str.")
            },
            "bool" => EvaluateBoolBinary(kind, returnType, left, right),
            "i8" => EvaluateNumericBinary<sbyte>(kind, returnType, left, right),
            "i16" => EvaluateNumericBinary<short>(kind, returnType, left, right),
            "i32" => EvaluateNumericBinary<int>(kind, returnType, left, right),
            "i64" => EvaluateNumericBinary<long>(kind, returnType, left, right),
            "isz" => EvaluateNumericBinary<nint>(kind, returnType, left, right),
            "u8" => EvaluateNumericBinary<byte>(kind, returnType, left, right),
            "u16" => EvaluateNumericBinary<ushort>(kind, returnType, left, right),
            "u32" => EvaluateNumericBinary<uint>(kind, returnType, left, right),
            "u64" => EvaluateNumericBinary<ulong>(kind, returnType, left, right),
            "usz" => EvaluateNumericBinary<nuint>(kind, returnType, left, right),
            "f16" => EvaluateNumericBinary<Half>(kind, returnType, left, right),
            "f32" => EvaluateNumericBinary<float>(kind, returnType, left, right),
            "f64" => EvaluateNumericBinary<double>(kind, returnType, left, right),
            _ => throw new NotSupportedException($"Unsupported binary operator '{kind}' for {concreteLeftType.Name}.")
        };
    }

    private PrimValue EvaluateBoolBinary(SyntaxKind kind, TypeSymbol returnType, PrimValue left, PrimValue right)
    {
        var a = (bool)left.Value;
        var b = (bool)right.Value;

        var result = kind switch
        {
            SyntaxKind.AmpersandAmpersandToken => a && b,
            SyntaxKind.PipePipeToken => a || b,
            _ => throw new NotSupportedException($"Unsupported binary operator '{kind}' for bool.")
        };

        return CreateValue(returnType, result);
    }

    private PrimValue EvaluateUnsignedUnary<T>(SyntaxKind kind, TypeSymbol returnType, PrimValue operand)
        where T : IBinaryInteger<T>
    {
        var value = (T)operand.Value;
        return kind switch
        {
            SyntaxKind.PlusToken => CreateValue(returnType, value),
            SyntaxKind.TildeToken => CreateValue(returnType, ~value),
            _ => throw new NotSupportedException($"Unsupported unary operator '{kind}' for {typeof(T).Name}.")
        };
    }

    private PrimValue EvaluateNumericUnary<T>(SyntaxKind kind, TypeSymbol returnType, PrimValue operand)
        where T : INumber<T>
    {
        var value = (T)operand.Value;
        return kind switch
        {
            SyntaxKind.PlusToken => CreateValue(returnType, value),
            SyntaxKind.MinusToken => CreateValue(returnType, -value),
            SyntaxKind.TildeToken when typeof(T) == typeof(sbyte) => CreateValue(returnType, (sbyte)(~(sbyte)(object)value)),
            SyntaxKind.TildeToken when typeof(T) == typeof(short) => CreateValue(returnType, (short)(~(short)(object)value)),
            SyntaxKind.TildeToken when typeof(T) == typeof(int) => CreateValue(returnType, ~(int)(object)value),
            SyntaxKind.TildeToken when typeof(T) == typeof(long) => CreateValue(returnType, ~(long)(object)value),
            SyntaxKind.TildeToken when typeof(T) == typeof(nint) => CreateValue(returnType, ~(nint)(object)value),
            _ => throw new NotSupportedException($"Unsupported unary operator '{kind}' for {typeof(T).Name}.")
        };
    }

    private PrimValue EvaluateNumericBinary<T>(SyntaxKind kind, TypeSymbol returnType, PrimValue left, PrimValue right)
        where T : INumber<T>
    {
        var a = (T)left.Value;
        var b = (T)right.Value;

        return kind switch
        {
            SyntaxKind.PlusToken => CreateValue(returnType, a + b),
            SyntaxKind.MinusToken => CreateValue(returnType, a - b),
            SyntaxKind.StarToken => CreateValue(returnType, a * b),
            SyntaxKind.SlashToken => CreateValue(returnType, a / b),
            SyntaxKind.PercentToken => CreateValue(returnType, a % b),
            SyntaxKind.StarStarToken => CreateValue(returnType, T.CreateChecked(Math.Pow(double.CreateChecked(a), double.CreateChecked(b)))),
            SyntaxKind.LessThanToken => CreateValue(returnType, a < b),
            SyntaxKind.LessThanEqualsToken => CreateValue(returnType, a <= b),
            SyntaxKind.GreaterThanToken => CreateValue(returnType, a > b),
            SyntaxKind.GreaterThanEqualsToken => CreateValue(returnType, a >= b),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(sbyte) => CreateValue(returnType, (sbyte)((sbyte)(object)a & (sbyte)(object)b)),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(short) => CreateValue(returnType, (short)((short)(object)a & (short)(object)b)),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(int) => CreateValue(returnType, (int)(object)a & (int)(object)b),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(long) => CreateValue(returnType, (long)(object)a & (long)(object)b),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(nint) => CreateValue(returnType, (nint)(object)a & (nint)(object)b),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(byte) => CreateValue(returnType, (byte)((byte)(object)a & (byte)(object)b)),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(ushort) => CreateValue(returnType, (ushort)((ushort)(object)a & (ushort)(object)b)),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(uint) => CreateValue(returnType, (uint)(object)a & (uint)(object)b),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(ulong) => CreateValue(returnType, (ulong)(object)a & (ulong)(object)b),
            SyntaxKind.AmpersandToken when typeof(T) == typeof(nuint) => CreateValue(returnType, (nuint)(object)a & (nuint)(object)b),
            SyntaxKind.PipeToken when typeof(T) == typeof(sbyte) => CreateValue(returnType, (sbyte)((sbyte)(object)a | (sbyte)(object)b)),
            SyntaxKind.PipeToken when typeof(T) == typeof(short) => CreateValue(returnType, (short)((short)(object)a | (short)(object)b)),
            SyntaxKind.PipeToken when typeof(T) == typeof(int) => CreateValue(returnType, (int)(object)a | (int)(object)b),
            SyntaxKind.PipeToken when typeof(T) == typeof(long) => CreateValue(returnType, (long)(object)a | (long)(object)b),
            SyntaxKind.PipeToken when typeof(T) == typeof(nint) => CreateValue(returnType, (nint)(object)a | (nint)(object)b),
            SyntaxKind.PipeToken when typeof(T) == typeof(byte) => CreateValue(returnType, (byte)((byte)(object)a | (byte)(object)b)),
            SyntaxKind.PipeToken when typeof(T) == typeof(ushort) => CreateValue(returnType, (ushort)((ushort)(object)a | (ushort)(object)b)),
            SyntaxKind.PipeToken when typeof(T) == typeof(uint) => CreateValue(returnType, (uint)(object)a | (uint)(object)b),
            SyntaxKind.PipeToken when typeof(T) == typeof(ulong) => CreateValue(returnType, (ulong)(object)a | (ulong)(object)b),
            SyntaxKind.PipeToken when typeof(T) == typeof(nuint) => CreateValue(returnType, (nuint)(object)a | (nuint)(object)b),
            SyntaxKind.HatToken when typeof(T) == typeof(sbyte) => CreateValue(returnType, (sbyte)((sbyte)(object)a ^ (sbyte)(object)b)),
            SyntaxKind.HatToken when typeof(T) == typeof(short) => CreateValue(returnType, (short)((short)(object)a ^ (short)(object)b)),
            SyntaxKind.HatToken when typeof(T) == typeof(int) => CreateValue(returnType, (int)(object)a ^ (int)(object)b),
            SyntaxKind.HatToken when typeof(T) == typeof(long) => CreateValue(returnType, (long)(object)a ^ (long)(object)b),
            SyntaxKind.HatToken when typeof(T) == typeof(nint) => CreateValue(returnType, (nint)(object)a ^ (nint)(object)b),
            SyntaxKind.HatToken when typeof(T) == typeof(byte) => CreateValue(returnType, (byte)((byte)(object)a ^ (byte)(object)b)),
            SyntaxKind.HatToken when typeof(T) == typeof(ushort) => CreateValue(returnType, (ushort)((ushort)(object)a ^ (ushort)(object)b)),
            SyntaxKind.HatToken when typeof(T) == typeof(uint) => CreateValue(returnType, (uint)(object)a ^ (uint)(object)b),
            SyntaxKind.HatToken when typeof(T) == typeof(ulong) => CreateValue(returnType, (ulong)(object)a ^ (ulong)(object)b),
            SyntaxKind.HatToken when typeof(T) == typeof(nuint) => CreateValue(returnType, (nuint)(object)a ^ (nuint)(object)b),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(sbyte) => CreateValue(returnType, (sbyte)((sbyte)(object)a << int.CreateChecked(b))),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(short) => CreateValue(returnType, (short)((short)(object)a << int.CreateChecked(b))),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(int) => CreateValue(returnType, (int)(object)a << int.CreateChecked(b)),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(long) => CreateValue(returnType, (long)(object)a << int.CreateChecked(b)),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(nint) => CreateValue(returnType, (nint)(object)a << int.CreateChecked(b)),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(byte) => CreateValue(returnType, (byte)((byte)(object)a << int.CreateChecked(b))),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(ushort) => CreateValue(returnType, (ushort)((ushort)(object)a << int.CreateChecked(b))),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(uint) => CreateValue(returnType, (uint)(object)a << int.CreateChecked(b)),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(ulong) => CreateValue(returnType, (ulong)(object)a << int.CreateChecked(b)),
            SyntaxKind.LessThanLessThanToken when typeof(T) == typeof(nuint) => CreateValue(returnType, (nuint)(object)a << int.CreateChecked(b)),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(sbyte) => CreateValue(returnType, (sbyte)((sbyte)(object)a >> int.CreateChecked(b))),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(short) => CreateValue(returnType, (short)((short)(object)a >> int.CreateChecked(b))),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(int) => CreateValue(returnType, (int)(object)a >> int.CreateChecked(b)),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(long) => CreateValue(returnType, (long)(object)a >> int.CreateChecked(b)),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(nint) => CreateValue(returnType, (nint)(object)a >> int.CreateChecked(b)),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(byte) => CreateValue(returnType, (byte)((byte)(object)a >> int.CreateChecked(b))),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(ushort) => CreateValue(returnType, (ushort)((ushort)(object)a >> int.CreateChecked(b))),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(uint) => CreateValue(returnType, (uint)(object)a >> int.CreateChecked(b)),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(ulong) => CreateValue(returnType, (ulong)(object)a >> int.CreateChecked(b)),
            SyntaxKind.GreaterThanGreaterThanToken when typeof(T) == typeof(nuint) => CreateValue(returnType, (nuint)(object)a >> int.CreateChecked(b)),
            _ => throw new NotSupportedException($"Unsupported binary operator '{kind}' for {typeof(T).Name}.")
        };
    }

    private PrimValue CreateDefaultValue(TypeSymbol type)
    {
        return type switch
        {
            ArrayTypeSymbol arrayType => new ArrayValue(arrayType, CreateDefaultElements(arrayType)),
            LambdaTypeSymbol lambdaType => throw new NotSupportedException($"Cannot synthesize a default runtime value for '{lambdaType.Name}'."),
            PointerTypeSymbol pointerType => throw new NotSupportedException($"Cannot synthesize a default runtime value for '{pointerType.Name}'."),
            StructTypeSymbol structType => CreateDefaultStructValue(structType),
            UnionTypeSymbol unionType => CreateDefaultUnionValue(unionType),
            _ => throw new NotSupportedException($"Unsupported runtime type '{type.Name}'.")
        };
    }

    private PrimValue[] CreateDefaultElements(ArrayTypeSymbol arrayType)
    {
        var length = arrayType.Length ?? 0;
        var elements = new PrimValue[length];
        for (var i = 0; i < length; i++)
        {
            elements[i] = CreateDefaultValue(arrayType.ElementType);
        }

        return elements;
    }

    private PrimValue CreateDefaultStructValue(StructTypeSymbol structType)
    {
        return structType.Name switch
        {
            "any" => CreateValue(structType, Unit.Value),
            "unknown" => CreateValue(structType, Unit.Value),
            "unit" => CreateValue(structType, Unit.Value),
            "str" => CreateValue(structType, string.Empty),
            "bool" => CreateValue(structType, false),
            "i8" => CreateValue(structType, (sbyte)0),
            "i16" => CreateValue(structType, (short)0),
            "i32" => CreateValue(structType, 0),
            "i64" => CreateValue(structType, 0L),
            "isz" => CreateValue(structType, (nint)0),
            "u8" => CreateValue(structType, (byte)0),
            "u16" => CreateValue(structType, (ushort)0),
            "u32" => CreateValue(structType, 0u),
            "u64" => CreateValue(structType, 0UL),
            "usz" => CreateValue(structType, (nuint)0),
            "f16" => CreateValue(structType, (Half)0),
            "f32" => CreateValue(structType, 0f),
            "f64" => CreateValue(structType, 0d),
            "never" => throw new InvalidOperationException("Cannot synthesize a runtime value for never."),
            _ => CreateDefaultUserStructValue(structType)
        };
    }

    private PrimValue CreateDefaultUserStructValue(StructTypeSymbol structType)
    {
        var prototype = GetStructValue(structType);
        var instance = new InstanceValue(prototype);
        foreach (var (symbol, value) in prototype.Members)
        {
            instance.Add(symbol, value);
        }

        return instance;
    }

    private PrimValue CreateDefaultUnionValue(UnionTypeSymbol unionType)
    {
        var memberType = unionType.Types.FirstOrDefault(type => type.MapsToUnit) ?? unionType.Types[0];
        return new UnionValue(unionType, CreateDefaultValue(memberType));
    }

    private PrimValue CreateValue(TypeSymbol type, object value)
    {
        return type switch
        {
            StructTypeSymbol structType => new InstanceValue(GetStructValue(structType), value),
            UnionTypeSymbol unionType when value is PrimValue primValue => new UnionValue(unionType, primValue),
            _ => throw new NotSupportedException($"Unsupported runtime value '{type.Name}'.")
        };
    }

    private StructValue GetStructValue(StructTypeSymbol structType)
    {
        var value = ResolveSymbolValue(structType);
        return value as StructValue
            ?? throw new InvalidOperationException($"Expected '{structType.Name}' to evaluate to a struct value.");
    }

    private ModuleValue ResolveModuleValue(ModuleSymbol module)
    {
        var value = ResolveSymbolValue(module);
        return value as ModuleValue
            ?? throw new InvalidOperationException($"Expected '{module.Name}' to evaluate to a module value.");
    }

    private Dictionary<Symbol, PrimValue> CaptureVisibleValues()
    {
        var captured = new Dictionary<Symbol, PrimValue>(ReferenceEqualityComparer.Instance);
        foreach (var frame in _frames.Reverse())
        {
            foreach (var (symbol, value) in frame.Values)
            {
                captured[symbol] = value;
            }
        }

        return captured;
    }

    private BoundTreeIndex GetOrCreateIndex(BoundNode root)
    {
        if (_indexes.TryGetValue(root, out var index))
        {
            return index;
        }

        index = BoundTreeIndex.Create(root);
        _indexes.Add(root, index);
        return index;
    }

    private BoundTreeIndex? CurrentIndex => _activeIndexes.Count > 0 ? _activeIndexes.Peek() : null;

    private static bool TryGetDeclaredSymbol(BoundNode node, out Symbol symbol)
    {
        switch (node)
        {
            case BoundModuleDeclaration module:
                symbol = module.Module;
                return true;
            case BoundPredefinedDeclaration predefined:
                symbol = predefined.Symbol;
                return true;
            case BoundPropertyDeclaration property:
                symbol = property.Property;
                return true;
            case BoundStructDeclaration @struct:
                symbol = @struct.StructSymbol;
                return true;
            case BoundVariableDeclaration variable:
                symbol = variable.VariableSymbol;
                return true;
            default:
                symbol = null!;
                return false;
        }
    }

    private sealed class EvaluationFrame
    {
        public EvaluationFrame(BoundNode owner, IReadOnlyDictionary<Symbol, PrimValue>? seed = null)
        {
            Owner = owner;
            Values = new Dictionary<Symbol, PrimValue>(ReferenceEqualityComparer.Instance);
            if (seed is null)
            {
                return;
            }

            foreach (var (symbol, value) in seed)
            {
                Values[symbol] = value;
            }
        }

        public BoundNode Owner { get; }
        public Dictionary<Symbol, PrimValue> Values { get; }
    }

    private sealed class BoundTreeIndex
    {
        private BoundTreeIndex(BoundNode root)
        {
            Root = root;
            Declarations = new Dictionary<Symbol, BoundDeclaration>(ReferenceEqualityComparer.Instance);
            Parameters = new Dictionary<Symbol, ParameterSlot>(ReferenceEqualityComparer.Instance);
        }

        public BoundNode Root { get; }
        public Dictionary<Symbol, BoundDeclaration> Declarations { get; }
        public Dictionary<Symbol, ParameterSlot> Parameters { get; }

        public static BoundTreeIndex Create(BoundNode root)
        {
            var index = new BoundTreeIndex(root);
            Index(root, index);
            return index;
        }

        private static void Index(BoundNode node, BoundTreeIndex index)
        {
            switch (node)
            {
                case BoundModuleDeclaration module:
                    index.Declarations[module.Module] = module;
                    break;
                case BoundPredefinedDeclaration predefined:
                    index.Declarations[predefined.Symbol] = predefined;
                    break;
                case BoundPropertyDeclaration property:
                    index.Declarations[property.Property] = property;
                    break;
                case BoundStructDeclaration @struct:
                    index.Declarations[@struct.StructSymbol] = @struct;
                    break;
                case BoundVariableDeclaration variable:
                    index.Declarations[variable.VariableSymbol] = variable;
                    break;
                case BoundLambdaExpression lambda:
                    for (var i = 0; i < lambda.Parameters.Length; i++)
                    {
                        index.Parameters[lambda.Parameters[i]] = new ParameterSlot(lambda, i);
                    }
                    break;
            }

            foreach (var child in node.Children().OfType<BoundNode>())
            {
                Index(child, index);
            }
        }
    }

    private readonly record struct ParameterSlot(BoundLambdaExpression Lambda, int Index);
}

