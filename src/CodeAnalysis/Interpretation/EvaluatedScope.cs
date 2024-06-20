using System.Collections;
using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Interpretation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;

internal class EvaluatedScope(EvaluatedScope? parent = null) : IEnumerable<PrimValue>
{
    protected Dictionary<Symbol, PrimValue>? Values { get; set; }

    public EvaluatedScope Parent { get => parent ?? GlobalEvaluatedScope.Instance; }

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!(Values ??= []).TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol)
    {
        var scope = this;
        var value = scope.Values?.GetValueOrDefault(symbol);
        if (value is not null)
            return value;

        do
        {
            scope = scope.Parent;
            value = scope.Values?.GetValueOrDefault(symbol);
            if (value is not null)
                return value;
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public IEnumerator<PrimValue> GetEnumerator()
    {
        foreach (var value in EnumerateValues(this))
            yield return value;

        static IEnumerable<PrimValue> EnumerateValues(EvaluatedScope? scope)
        {
            if (scope is null) yield break;
            if (scope.Values is not null)
            {
                foreach (var (_, value) in scope.Values)
                    yield return value;
            }
            foreach (var value in EnumerateValues(scope.Parent))
                yield return value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class GlobalEvaluatedScope : EvaluatedScope
{
    private static GlobalEvaluatedScope? s_instance;
    public static GlobalEvaluatedScope Instance
    {
        get
        {
            if (s_instance is null)
                Interlocked.CompareExchange(ref s_instance, new GlobalEvaluatedScope(), null);
            return s_instance;
        }
    }

    private GlobalEvaluatedScope() : base()
    {
        var values = MapPredefinedTypes();

        Any = (StructValue)values[GlobalBoundScope.Instance.Any];
        Unknown = (StructValue)values[GlobalBoundScope.Instance.Unknown];
        Never = (StructValue)values[GlobalBoundScope.Instance.Never];
        Unit = (StructValue)values[GlobalBoundScope.Instance.Unit];
        Type = (StructValue)values[GlobalBoundScope.Instance.Type];
        Str = (StructValue)values[GlobalBoundScope.Instance.Str];
        Bool = (StructValue)values[GlobalBoundScope.Instance.Bool];
        I8 = (StructValue)values[GlobalBoundScope.Instance.I8];
        I16 = (StructValue)values[GlobalBoundScope.Instance.I16];
        I32 = (StructValue)values[GlobalBoundScope.Instance.I32];
        I64 = (StructValue)values[GlobalBoundScope.Instance.I64];
        I128 = (StructValue)values[GlobalBoundScope.Instance.I128];
        ISize = (StructValue)values[GlobalBoundScope.Instance.ISize];
        U8 = (StructValue)values[GlobalBoundScope.Instance.U8];
        U16 = (StructValue)values[GlobalBoundScope.Instance.U16];
        U32 = (StructValue)values[GlobalBoundScope.Instance.U32];
        U64 = (StructValue)values[GlobalBoundScope.Instance.U64];
        U128 = (StructValue)values[GlobalBoundScope.Instance.U128];
        USize = (StructValue)values[GlobalBoundScope.Instance.USize];
        F16 = (StructValue)values[GlobalBoundScope.Instance.F16];
        F32 = (StructValue)values[GlobalBoundScope.Instance.F32];
        F64 = (StructValue)values[GlobalBoundScope.Instance.F64];
        F80 = (StructValue)values[GlobalBoundScope.Instance.F80];
        F128 = (StructValue)values[GlobalBoundScope.Instance.F128];

        Values = values;

        static Dictionary<Symbol, PrimValue> MapPredefinedTypes()
        {
            var global = GlobalBoundScope.Instance;
            var map = new Dictionary<Symbol, PrimValue>();
            foreach (var name in PredefinedTypeNames.All)
            {
                var symbol = global.Lookup(name) as StructSymbol
                    ?? throw new UnreachableException(DiagnosticMessage.UndefinedType(name));
                map[symbol] = new StructValue(symbol.Type);
                // TODO: Create members for struct.
            }
            ((StructValue)map[global.Str])
                .AddEqualityOperators<string>(global.Str)
                .AddMembers(s =>
                {
                    var add = global.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, global.Str.Type, global.Str.Type, global.Str.Type).Single();
                    s.SetOperator(
                        new OperatorSymbol(
                            SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                            add,
                            global.Str),
                        new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                            new VariableValue(a.Type, (string)a.Value + (string)b.Value)));
                    var addStr = global.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, global.Str.Type, PredefinedTypes.Any, global.Str.Type).Single();
                    s.SetOperator(
                        new OperatorSymbol(
                            SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                            addStr,
                            global.Str),
                        new FunctionValue(addStr.Type, (PrimValue a, PrimValue b) =>
                            new VariableValue(a.Type, (string)a.Value + b.Value)));
                    var addAny = global.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, PredefinedTypes.Any, global.Str.Type, global.Str.Type).Single();
                    s.SetOperator(
                        new OperatorSymbol(
                            SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                            addAny,
                            global.Str),
                        new FunctionValue(addAny.Type, (PrimValue a, PrimValue b) =>
                            new VariableValue(a.Type, a.Value + (string)b.Value)));
                });
            ((StructValue)map[global.Bool])
                .AddEqualityOperators<bool>(global.Bool)
                .AddLogicalOperators(global.Bool);
            ((StructValue)map[global.I8])
                .AddEqualityOperators<sbyte>(global.I8)
                .AddComparisonOperators<sbyte>(global.I8)
                .AddBitwiseOperators<sbyte>(global.I8)
                .AddMathOperators<sbyte>(global.I8);
            ((StructValue)map[global.I16])
                .AddEqualityOperators<short>(global.I16)
                .AddComparisonOperators<short>(global.I16)
                .AddBitwiseOperators<short>(global.I16)
                .AddMathOperators<short>(global.I16);
            ((StructValue)map[global.I32])
                .AddEqualityOperators<int>(global.I32)
                .AddComparisonOperators<int>(global.I32)
                .AddBitwiseOperators<int>(global.I32)
                .AddMathOperators<int>(global.I32);
            ((StructValue)map[global.I64])
                .AddEqualityOperators<long>(global.I64)
                .AddComparisonOperators<long>(global.I64)
                .AddBitwiseOperators<long>(global.I64)
                .AddMathOperators<long>(global.I64);
            ((StructValue)map[global.I128])
                .AddEqualityOperators<BigInteger>(global.I64)
                .AddComparisonOperators<BigInteger>(global.I64)
                .AddBitwiseOperators<BigInteger>(global.I64)
                .AddMathOperators<BigInteger>(global.I64);
            ((StructValue)map[global.ISize])
                .AddEqualityOperators<nint>(global.I64)
                .AddComparisonOperators<nint>(global.I64)
                .AddBitwiseOperators<nint>(global.I64)
                .AddMathOperators<nint>(global.I64);
            ((StructValue)map[global.U8])
                .AddEqualityOperators<byte>(global.U8)
                .AddComparisonOperators<byte>(global.U8)
                .AddBitwiseOperators<byte>(global.U8)
                .AddMathOperators<byte>(global.U8);
            ((StructValue)map[global.U16])
                .AddEqualityOperators<ushort>(global.U16)
                .AddComparisonOperators<ushort>(global.U16)
                .AddBitwiseOperators<ushort>(global.U16)
                .AddMathOperators<ushort>(global.U16);
            ((StructValue)map[global.U32])
                .AddEqualityOperators<uint>(global.U32)
                .AddComparisonOperators<uint>(global.U32)
                .AddBitwiseOperators<uint>(global.U32)
                .AddMathOperators<uint>(global.U32);
            ((StructValue)map[global.U64])
                .AddEqualityOperators<ulong>(global.U64)
                .AddComparisonOperators<ulong>(global.U64)
                .AddBitwiseOperators<ulong>(global.U64)
                .AddMathOperators<ulong>(global.U64);
            ((StructValue)map[global.U128])
                .AddEqualityOperators<BigInteger>(global.U64)
                .AddComparisonOperators<BigInteger>(global.U64)
                .AddBitwiseOperators<BigInteger>(global.U64)
                .AddMathOperators<BigInteger>(global.U64);
            ((StructValue)map[global.USize])
                .AddEqualityOperators<nuint>(global.U64)
                .AddComparisonOperators<nuint>(global.U64)
                .AddBitwiseOperators<nuint>(global.U64)
                .AddMathOperators<nuint>(global.U64);
            ((StructValue)map[global.F16])
                .AddEqualityOperators<Half>(global.F16)
                .AddComparisonOperators<Half>(global.F16)
                .AddMathOperators<Half>(global.F16);
            ((StructValue)map[global.F32])
                .AddEqualityOperators<float>(global.F32)
                .AddComparisonOperators<float>(global.F32)
                .AddMathOperators<float>(global.F32);
            ((StructValue)map[global.F64])
                .AddEqualityOperators<double>(global.F64)
                .AddComparisonOperators<double>(global.F64)
                .AddMathOperators<double>(global.F64);
            // TODO: Fix these types using double.
            ((StructValue)map[global.F80])
                .AddEqualityOperators<double>(global.F80)
                .AddComparisonOperators<double>(global.F80)
                .AddMathOperators<double>(global.F80);
            ((StructValue)map[global.F128])
                .AddEqualityOperators<double>(global.F128)
                .AddComparisonOperators<double>(global.F128)
                .AddMathOperators<double>(global.F128);
            return map;
        }
    }

    public StructValue Any { get; }
    public StructValue Unknown { get; }
    public StructValue Never { get; }
    public StructValue Unit { get; }
    public StructValue Type { get; }
    public StructValue Str { get; }
    public StructValue Bool { get; }
    public StructValue I8 { get; }
    public StructValue I16 { get; }
    public StructValue I32 { get; }
    public StructValue I64 { get; }
    public StructValue I128 { get; }
    public StructValue ISize { get; }
    public StructValue U8 { get; }
    public StructValue U16 { get; }
    public StructValue U32 { get; }
    public StructValue U64 { get; }
    public StructValue U128 { get; }
    public StructValue USize { get; }
    public StructValue F16 { get; }
    public StructValue F32 { get; }
    public StructValue F64 { get; }
    public StructValue F80 { get; }
    public StructValue F128 { get; }
}

file static class StructValueExtensions
{
    public static StructValue AddMembers(this StructValue s, Action<StructValue> add)
    {
        add(s);
        return s;
    }

    public static StructValue AddMathOperators<T>(this StructValue s, StructSymbol symbol) where T : INumber<T>
    {
        var unaryPlus = symbol.Type.GetUnaryOperators(SyntaxKind.UnaryPlusOperator, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                unaryPlus,
                symbol),
            new FunctionValue(unaryPlus.Type, (PrimValue a) => new VariableValue(a.Type, +(T)a.Value)));
        var unaryMinus = symbol.Type.GetUnaryOperators(SyntaxKind.UnaryMinusOperator, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.MinusToken),
                unaryMinus,
                symbol),
            new FunctionValue(unaryMinus.Type, (PrimValue a) => new VariableValue(a.Type, -(T)a.Value)));
        var add = symbol.Type.GetBinaryOperators(SyntaxKind.AddOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                add,
                symbol),
            new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value + (T)b.Value)));
        var subtract = symbol.Type.GetBinaryOperators(SyntaxKind.SubtractOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.MinusToken),
                subtract,
                symbol),
            new FunctionValue(subtract.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value - (T)b.Value)));
        var multiply = symbol.Type.GetBinaryOperators(SyntaxKind.MultiplyOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.StarToken),
                multiply,
                symbol),
            new FunctionValue(multiply.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value * (T)b.Value)));
        var divide = symbol.Type.GetBinaryOperators(SyntaxKind.DivideOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.SlashToken),
                divide,
                symbol),
            new FunctionValue(divide.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value / (T)b.Value)));
        var modulo = symbol.Type.GetBinaryOperators(SyntaxKind.ModuloOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PercentToken),
                modulo,
                symbol),
            new FunctionValue(modulo.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value % (T)b.Value)));
        var power = symbol.Type.GetBinaryOperators(SyntaxKind.PowerOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.StarStarToken),
                power,
                symbol),
            new FunctionValue(power.Type, (PrimValue a, PrimValue b) => T.CreateTruncating(Math.Pow(double.CreateTruncating((T)a.Value), double.CreateTruncating((T)b.Value)))));
        return s;
    }

    public static StructValue AddBitwiseOperators<T>(this StructValue s, StructSymbol symbol) where T : IBinaryInteger<T>, IShiftOperators<T, int, T>
    {
        var onesComplement = symbol.Type.GetUnaryOperators(SyntaxKind.OnesComplementOperator, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.TildeToken),
                onesComplement,
                symbol),
            new FunctionValue(onesComplement.Type, (PrimValue a) => new VariableValue(a.Type, ~(T)a.Value)));
        var bitwiseAnd = symbol.Type.GetBinaryOperators(SyntaxKind.BitwiseAndOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.AmpersandToken),
                bitwiseAnd,
                symbol),
            new FunctionValue(bitwiseAnd.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value & (T)b.Value)));
        var bitwiseOr = symbol.Type.GetBinaryOperators(SyntaxKind.BitwiseOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PipeToken),
                bitwiseOr,
                symbol),
            new FunctionValue(bitwiseOr.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value | (T)b.Value)));
        var exclusiveOr = symbol.Type.GetBinaryOperators(SyntaxKind.ExclusiveOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.HatToken),
                exclusiveOr,
                symbol),
            new FunctionValue(exclusiveOr.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value ^ (T)b.Value)));
        var leftShift = symbol.Type.GetBinaryOperators(SyntaxKind.LeftShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.LessThanLessThanToken),
                leftShift,
                symbol),
            new FunctionValue(leftShift.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value << int.CreateTruncating((T)b.Value))));
        var rightShift = symbol.Type.GetBinaryOperators(SyntaxKind.RightShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.BangToken),
                rightShift,
                symbol),
            new FunctionValue(rightShift.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(a.Type, (T)a.Value >> int.CreateTruncating((T)b.Value))));
        return s;
    }

    public static StructValue AddEqualityOperators<T>(this StructValue s, StructSymbol symbol)
    {
        var equals = symbol.Type.GetBinaryOperators(SyntaxKind.EqualsOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.EqualsEqualsToken),
                equals,
                symbol),
            new FunctionValue(equals.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, ((T)a.Value).Equals(b.Value))));
        var notEquals = symbol.Type.GetBinaryOperators(SyntaxKind.NotEqualsOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.BangEqualsToken),
                notEquals,
                symbol),
            new FunctionValue(notEquals.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, !((T)a.Value).Equals(b.Value))));
        return s;
    }

    public static StructValue AddComparisonOperators<T>(this StructValue s, StructSymbol symbol) where T : IComparisonOperators<T, T, bool>
    {
        var lessThan = symbol.Type.GetBinaryOperators(SyntaxKind.LessThanOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.LessThanToken),
                lessThan,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = symbol.Type.GetBinaryOperators(SyntaxKind.LessThanOrEqualOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.LessThanEqualsToken),
                lessThanOrEqual,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, (T)a.Value <= (T)b.Value)));
        var greaterThan = symbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.GreaterThanToken),
                greaterThan,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = symbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanOrEqualOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.GreaterThanEqualsToken),
                greaterThanOrEqual,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, (T)a.Value >= (T)b.Value)));
        return s;
    }

    public static StructValue AddLogicalOperators(this StructValue s, StructSymbol symbol)
    {
        var not = symbol.Type.GetUnaryOperators(SyntaxKind.NotOperator, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.BangToken),
                not,
                symbol),
            new FunctionValue(not.Type, (PrimValue a) =>
                new VariableValue(PredefinedTypes.Bool, !(bool)a.Value)));
        var logicalAnd = symbol.Type.GetBinaryOperators(SyntaxKind.LogicalAndOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.AmpersandAmpersandToken),
                logicalAnd,
                symbol),
            new FunctionValue(logicalAnd.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, (bool)a.Value && (bool)b.Value)));
        var logicalOr = symbol.Type.GetBinaryOperators(SyntaxKind.LogicalOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PipePipeToken),
                logicalOr,
                symbol),
            new FunctionValue(logicalOr.Type, (PrimValue a, PrimValue b) =>
                new VariableValue(PredefinedTypes.Bool, (bool)a.Value || (bool)b.Value)));
        return s;
    }

    public static StructValue AddImplicitConversion(this StructValue s, params ReadOnlySpan<PrimType> targetTypes)
    {
        _ = targetTypes;
        return s;
    }

    public static StructValue AddExplicitConversion(this StructValue s, params ReadOnlySpan<PrimType> targetTypes)
    {
        _ = targetTypes;
        return s;
    }
}
