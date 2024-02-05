using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Operators;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Text;
using CodeAnalysis.Types;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Expr = System.Linq.Expressions.Expression;

namespace CodeAnalysis;

internal static class Interpreter
{
    internal static Result Interpret(Environment environment, BoundProgram program)
    {
        var context = new InterpretContext(Unit.Value, environment, []);

        foreach (var node in program.Nodes)
            InterpretBoundNode(context, node);

        return new Result(context.Value, context.Diagnostics);
    }

    private static object InterpretBoundNode(InterpretContext context, BoundNode boundNode)
    {
        return boundNode.NodeKind switch
        {
            //BoundNodeKind.GlobalExpression => InterpretGlobalExpression(context, (BoundGlobalExpression)boundNode),
            //BoundNodeKind.BlockExpression => InterpretBlockExpression(context, (BoundBlockExpression)boundNode),
            BoundNodeKind.LiteralExpression => InterpretLiteralExpression(context, (BoundLiteralExpression)boundNode),
            //BoundNodeKind.UnaryExpression => InterpretUnaryExpression(context, (BoundUnaryExpression)boundNode),
            BoundNodeKind.ArgumentList => InterpretArgumentListExpression(context, (BoundArgumentListExpression)boundNode),
            BoundNodeKind.BinaryExpression => InterpretBinaryExpression(context, (BoundBinaryExpression)boundNode),
            BoundNodeKind.DeclarationExpression => InterpretDeclarationExpression(context, (BoundDeclarationExpression)boundNode),
            //BoundNodeKind.AssignmentExpression => InterpretAssignmentExpression(context, (BoundAssignmentExpression)boundNode),
            BoundNodeKind.NameExpression => InterpretNameExpression(context, (BoundNameExpression)boundNode),
            //BoundNodeKind.ForExpression => InterpretForExpression(context, (BoundForExpression)boundNode),
            //BoundNodeKind.IfElseExpression => InterpretIfElseExpression(context, (BoundIfElseExpression)boundNode),
            //BoundNodeKind.WhileExpression => InterpretWhileExpression(context, (BoundWhileExpression)boundNode),
            //BoundNodeKind.BreakExpression => InterpretBreakExpression(context, (BoundBreakExpression)boundNode),
            //BoundNodeKind.ContinueExpression => InterpretContinueExpression(context, (BoundContinueExpression)boundNode),
            //BoundNodeKind.ReturnExpression => InterpretReturnExpression(context, (BoundReturnExpression)boundNode),
            //BoundNodeKind.ConvertExpression => throw new NotImplementedException(),
            //BoundNodeKind.NeverExpression => throw new NotImplementedException(),
            BoundNodeKind.Symbol => throw new NotImplementedException(),
            BoundNodeKind.Operator => throw new NotImplementedException(),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundNodeKind)} '{boundNode.NodeKind}'"),
        };
    }

    private static object InterpretLiteralExpression(InterpretContext context, BoundLiteralExpression boundNode)
    {
        return context.Value = boundNode.Value ?? Unit.Value;
    }

    private static object InterpretArgumentListExpression(InterpretContext context, BoundArgumentListExpression boundNode)
    {
        var args = new object?[boundNode.Arguments.Count];
        for (int i = 0; i < args.Length; ++i)
            args[i] = InterpretBoundNode(context, boundNode.Arguments[i]);
        return context.Value = args;
    }

    private static object InterpretBinaryExpression(InterpretContext context, BoundBinaryExpression boundNode)
    {
        dynamic left = InterpretBoundNode(context, boundNode.Left);
        dynamic right = InterpretBoundNode(context, boundNode.Right);

        return context.Value = boundNode.Operator.OperatorKind switch
        {
            OperatorKind.Add => left + right,
            OperatorKind.Subtract => left - right,
            OperatorKind.Multiply => left * right,
            OperatorKind.Divide => left / right,
            OperatorKind.Modulo => left % right,
            OperatorKind.Exponent => Convert.ChangeType(Math.Pow((double)left, (double)right), left?.GetType() ?? typeof(object)),
            OperatorKind.And => left & right,
            OperatorKind.Or => left | right,
            OperatorKind.ExclusiveOr => left ^ right,
            OperatorKind.LeftShift => left << right,
            OperatorKind.RightShift => left >> right,
            OperatorKind.Equal => left == right,
            OperatorKind.NotEqual => left != right,
            OperatorKind.LessThan => left < right,
            OperatorKind.LessThanOrEqual => left <= right,
            OperatorKind.GreaterThan => left > right,
            OperatorKind.GreaterThanOrEqual => left >= right,
            OperatorKind.AndAlso => left && right,
            OperatorKind.OrElse => left || right,
            OperatorKind.NullCoalescence => left ?? right,
            OperatorKind.Call => left.DynamicInvoke(right),
            _ => throw new UnreachableException($"Unexpected {nameof(OperatorKind)} '{boundNode.Operator.OperatorKind}'"),
        } ?? Unit.Value;
    }

    private static Symbol InterpretDeclarationExpression(InterpretContext context, BoundDeclarationExpression boundNode)
    {
        switch (boundNode.DeclarationKind)
        {
            case DeclarationKind.Variable:
                context.Environment.Declare(boundNode.Symbol, InterpretBoundNode(context, boundNode.Expression));
                return boundNode.Symbol;

            case DeclarationKind.Function:
                context.Environment.Declare(boundNode.Symbol, CreateFunc(context, (FunctionType)boundNode.Symbol.Type, boundNode.Expression));
                return boundNode.Symbol;

            default:
                throw new UnreachableException($"Unexpected {nameof(DeclarationKind)} '{boundNode.DeclarationKind}'");
        }

        static Delegate CreateFunc(InterpretContext context, FunctionType function, BoundExpression expression)
        {
            System.Linq.Expressions.Expression<Action<InterpretContext, Symbol, object>> DeclareSymbol =
                (InterpretContext context, Symbol symbol, object value) => context.Environment.Declare(symbol, value);

            var pushEnv = typeof(InterpretContext).GetMethod(nameof(InterpretContext.PushEnvironment))!;
            var popEnv = typeof(InterpretContext).GetMethod(nameof(InterpretContext.PopEnvironment))!;
            var environment = typeof(InterpretContext).GetProperty(nameof(InterpretContext.Environment))!;
            var declare = typeof(Environment).GetMethod(nameof(Environment.Declare))!;
            var symbol = typeof(Symbol).GetConstructor([typeof(string), typeof(PrimType), typeof(bool)])!;
            var interpret = typeof(Interpreter).GetMethod(nameof(InterpretBoundNode), BindingFlags.NonPublic | BindingFlags.Static)!;

            var parameters = function.Parameters.Select(p => Expr.Parameter(typeof(object), p.Name)).ToArray();

            var result = Expr.Variable(typeof(object), "result");
            var ctx = Expr.Constant(context, typeof(InterpretContext));
            var node = Expr.Constant(expression, typeof(BoundExpression));
            var env = Expr.Property(ctx, environment);

            var body = Expr.Block(
                [result],
                Expr.Call(ctx, pushEnv),
                Expr.Block(function.Parameters.Select((p, i) => Expr.Call(env, declare, Expr.New(symbol, Expr.Constant(p.Name), Expr.Constant(p.Type), Expr.Constant(false)), parameters[i]))),
                Expr.Assign(result, Expr.Call(interpret, ctx, node)),
                Expr.Call(ctx, popEnv),
                result
            );
            var lambda = Expr.Lambda(body, parameters);
            var func = lambda.Compile();
            return func;
        }
    }

    private static object InterpretNameExpression(InterpretContext context, BoundNameExpression boundNode)
    {
        return context.InterpretLocal(boundNode.Symbol);
    }
}

internal sealed record class Unit
{
    public static readonly Unit Value = new();
    private Unit() { }
    public override string ToString() => PredefinedSymbolNames.Unit;
}

internal sealed record class InterpretContext(object Value, Environment Environment, DiagnosticBag Diagnostics)
{
    public object Value { get; set; } = Value;

    public Environment Environment { get; set; } = Environment;

    public object InterpretLocal(Symbol symbol) => Value = Environment.Lookup(symbol) ?? Unit.Value;

    public void PushEnvironment() => Environment = Environment.CreateScoped();
    public void PopEnvironment() => Environment = Environment.Parent ?? throw new InvalidOperationException("Invalid environment stack");
}

internal sealed record class Environment : IEnumerable<object?>
{
    private Dictionary<Symbol, object?>? _symbols;

    private Environment() { }

    public Environment? Parent { get; init; }

    public bool IsGlobal { get => Parent is null; }

    public Environment CreateScoped() => new() { Parent = this };

    public void Declare(Symbol symbol, object value)
    {
        if (!(_symbols ??= []).TryAdd(symbol, value))
            throw new UnreachableException($"{symbol.Name} redeclared");
    }

    public object? Lookup(Symbol symbol)
    {
        if (_symbols is not null && _symbols.TryGetValue(symbol, out var value))
            return value;

        if (Parent is not null)
            return Parent.Lookup(symbol);

        throw new UnreachableException($"{nameof(Parent)} was null");
    }

    public IEnumerator<object?> GetEnumerator() => _symbols?.Values.GetEnumerator() ?? Enumerable.Empty<object?>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Environment CreateGlobal()
    {
        var scope = new Environment();

        foreach (var symbol in PredefinedSymbols.All)
            scope.Declare(symbol, symbol.Type switch
            {
                TypeType type => type,
                FunctionType func => symbol.Name switch
                {
                    PredefinedSymbolNames.ScanLn => new Func<object?>(Console.ReadLine),
                    PredefinedSymbolNames.PrintLn => new Func<object?, object?>(obj =>
                    {
                        Console.WriteLine(obj);
                        return null;
                    }),
                    _ => throw new UnreachableException($"Unexpected predefined function '{symbol.Name}'")
                },
                _ => throw new UnreachableException($"Unexpected predefined symbol '{symbol.Name}'")
            });

        return scope;
    }
}