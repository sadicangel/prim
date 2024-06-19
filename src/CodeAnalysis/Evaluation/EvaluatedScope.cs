using System.Collections;
using System.Diagnostics;
using System.Numerics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Evaluation.Values;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;

internal class EvaluatedScope(EvaluatedScope? parent = null) : IEnumerable<PrimValue>
{
    protected Dictionary<Symbol, PrimValue>? Values { get; set; }

    public EvaluatedScope? Parent { get; } = parent;

    public void Declare(Symbol symbol, PrimValue value)
    {
        if (!(Values ??= []).TryAdd(symbol, value))
            throw new UnreachableException(DiagnosticMessage.SymbolRedeclaration(symbol.Name));
    }

    public PrimValue Lookup(Symbol symbol) => Values?.GetValueOrDefault(symbol) ?? Parent?.Lookup(symbol)
        ?? throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));

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

    public static EvaluatedScope FromGlobalBoundScope(GlobalBoundScope boundScope)
    {
        return new EvaluatedScope()
        {
            Values = MapPredefinedTypes(boundScope)
        };

        static Dictionary<Symbol, PrimValue> MapPredefinedTypes(GlobalBoundScope boundScope)
        {
            var map = new Dictionary<Symbol, PrimValue>();
            foreach (var name in PredefinedTypeNames.All)
            {
                var symbol = boundScope.Lookup(name) as StructSymbol
                    ?? throw new UnreachableException(DiagnosticMessage.UndefinedType(name));
                map[symbol] = new StructValue(symbol.Type);
                // TODO: Create members for struct.
            }
            ((StructValue)map[boundScope.Str])
                .AddEqualityOperators<string>(boundScope.Str)
                .AddMembers(s =>
                {
                    var addStr = boundScope.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, boundScope.Str.Type, PredefinedTypes.Any, boundScope.Str.Type).Single();
                    s.SetOperator(
                        new OperatorSymbol(
                            SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                            addStr,
                            boundScope.Str),
                        new FunctionValue(addStr.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(a.Type, (string)a.Value + b.Value)));
                    var addAny = boundScope.Str.Type.GetBinaryOperators(SyntaxKind.AddOperator, PredefinedTypes.Any, boundScope.Str.Type, boundScope.Str.Type).Single();
                    s.SetOperator(
                        new OperatorSymbol(
                            SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                            addAny,
                            boundScope.Str),
                        new FunctionValue(addAny.Type, (PrimValue a, PrimValue b) =>
                            new LiteralValue(a.Type, a.Value + (string)b.Value)));
                });
            ((StructValue)map[boundScope.Bool])
                .AddEqualityOperators<bool>(boundScope.Bool)
                .AddLogicalOperators(boundScope.Bool);
            ((StructValue)map[boundScope.I8])
                .AddEqualityOperators<sbyte>(boundScope.I8)
                .AddComparisonOperators<sbyte>(boundScope.I8)
                .AddBitwiseOperators<sbyte>(boundScope.I8)
                .AddMathOperators<sbyte>(boundScope.I8);
            ((StructValue)map[boundScope.I16])
                .AddEqualityOperators<short>(boundScope.I16)
                .AddComparisonOperators<short>(boundScope.I16)
                .AddBitwiseOperators<short>(boundScope.I16)
                .AddMathOperators<short>(boundScope.I16);
            ((StructValue)map[boundScope.I32])
                .AddEqualityOperators<int>(boundScope.I32)
                .AddComparisonOperators<int>(boundScope.I32)
                .AddBitwiseOperators<int>(boundScope.I32)
                .AddMathOperators<int>(boundScope.I32);
            ((StructValue)map[boundScope.I64])
                .AddEqualityOperators<long>(boundScope.I64)
                .AddComparisonOperators<long>(boundScope.I64)
                .AddBitwiseOperators<long>(boundScope.I64)
                .AddMathOperators<long>(boundScope.I64);
            ((StructValue)map[boundScope.I128])
                .AddEqualityOperators<BigInteger>(boundScope.I64)
                .AddComparisonOperators<BigInteger>(boundScope.I64)
                .AddBitwiseOperators<BigInteger>(boundScope.I64)
                .AddMathOperators<BigInteger>(boundScope.I64);
            ((StructValue)map[boundScope.ISize])
                .AddEqualityOperators<nint>(boundScope.I64)
                .AddComparisonOperators<nint>(boundScope.I64)
                .AddBitwiseOperators<nint>(boundScope.I64)
                .AddMathOperators<nint>(boundScope.I64);
            ((StructValue)map[boundScope.U8])
                .AddEqualityOperators<byte>(boundScope.U8)
                .AddComparisonOperators<byte>(boundScope.U8)
                .AddBitwiseOperators<byte>(boundScope.U8)
                .AddMathOperators<byte>(boundScope.U8);
            ((StructValue)map[boundScope.U16])
                .AddEqualityOperators<ushort>(boundScope.U16)
                .AddComparisonOperators<ushort>(boundScope.U16)
                .AddBitwiseOperators<ushort>(boundScope.U16)
                .AddMathOperators<ushort>(boundScope.U16);
            ((StructValue)map[boundScope.U32])
                .AddEqualityOperators<uint>(boundScope.U32)
                .AddComparisonOperators<uint>(boundScope.U32)
                .AddBitwiseOperators<uint>(boundScope.U32)
                .AddMathOperators<uint>(boundScope.U32);
            ((StructValue)map[boundScope.U64])
                .AddEqualityOperators<ulong>(boundScope.U64)
                .AddComparisonOperators<ulong>(boundScope.U64)
                .AddBitwiseOperators<ulong>(boundScope.U64)
                .AddMathOperators<ulong>(boundScope.U64);
            ((StructValue)map[boundScope.U128])
                .AddEqualityOperators<BigInteger>(boundScope.U64)
                .AddComparisonOperators<BigInteger>(boundScope.U64)
                .AddBitwiseOperators<BigInteger>(boundScope.U64)
                .AddMathOperators<BigInteger>(boundScope.U64);
            ((StructValue)map[boundScope.USize])
                .AddEqualityOperators<nuint>(boundScope.U64)
                .AddComparisonOperators<nuint>(boundScope.U64)
                .AddBitwiseOperators<nuint>(boundScope.U64)
                .AddMathOperators<nuint>(boundScope.U64);
            ((StructValue)map[boundScope.F16])
                .AddEqualityOperators<Half>(boundScope.F16)
                .AddComparisonOperators<Half>(boundScope.F16)
                .AddMathOperators<Half>(boundScope.F16);
            ((StructValue)map[boundScope.F32])
                .AddEqualityOperators<float>(boundScope.F32)
                .AddComparisonOperators<float>(boundScope.F32)
                .AddMathOperators<float>(boundScope.F32);
            ((StructValue)map[boundScope.F64])
                .AddEqualityOperators<double>(boundScope.F64)
                .AddComparisonOperators<double>(boundScope.F64)
                .AddMathOperators<double>(boundScope.F64);
            // TODO: Fix these types using double.
            ((StructValue)map[boundScope.F80])
                .AddEqualityOperators<double>(boundScope.F80)
                .AddComparisonOperators<double>(boundScope.F80)
                .AddMathOperators<double>(boundScope.F80);
            ((StructValue)map[boundScope.F128])
                .AddEqualityOperators<double>(boundScope.F128)
                .AddComparisonOperators<double>(boundScope.F128)
                .AddMathOperators<double>(boundScope.F128);
            return map;
        }
    }
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
            new FunctionValue(unaryPlus.Type, (PrimValue a) => new LiteralValue(a.Type, +(T)a.Value)));
        var unaryMinus = symbol.Type.GetUnaryOperators(SyntaxKind.UnaryMinusOperator, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.MinusToken),
                unaryMinus,
                symbol),
            new FunctionValue(unaryMinus.Type, (PrimValue a) => new LiteralValue(a.Type, -(T)a.Value)));
        var add = symbol.Type.GetBinaryOperators(SyntaxKind.AddOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PlusToken),
                add,
                symbol),
            new FunctionValue(add.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value + (T)b.Value)));
        var subtract = symbol.Type.GetBinaryOperators(SyntaxKind.SubtractOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.MinusToken),
                subtract,
                symbol),
            new FunctionValue(subtract.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value - (T)b.Value)));
        var multiply = symbol.Type.GetBinaryOperators(SyntaxKind.MultiplyOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.StarToken),
                multiply,
                symbol),
            new FunctionValue(multiply.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value * (T)b.Value)));
        var divide = symbol.Type.GetBinaryOperators(SyntaxKind.DivideOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.SlashToken),
                divide,
                symbol),
            new FunctionValue(divide.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value / (T)b.Value)));
        var modulo = symbol.Type.GetBinaryOperators(SyntaxKind.ModuloOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PercentToken),
                modulo,
                symbol),
            new FunctionValue(modulo.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value % (T)b.Value)));
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
            new FunctionValue(onesComplement.Type, (PrimValue a) => new LiteralValue(a.Type, ~(T)a.Value)));
        var bitwiseAnd = symbol.Type.GetBinaryOperators(SyntaxKind.BitwiseAndOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.AmpersandToken),
                bitwiseAnd,
                symbol),
            new FunctionValue(bitwiseAnd.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value & (T)b.Value)));
        var bitwiseOr = symbol.Type.GetBinaryOperators(SyntaxKind.BitwiseOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PipeToken),
                bitwiseOr,
                symbol),
            new FunctionValue(bitwiseOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value | (T)b.Value)));
        var exclusiveOr = symbol.Type.GetBinaryOperators(SyntaxKind.ExclusiveOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.HatToken),
                exclusiveOr,
                symbol),
            new FunctionValue(exclusiveOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value ^ (T)b.Value)));
        var leftShift = symbol.Type.GetBinaryOperators(SyntaxKind.LeftShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.LessThanLessThanToken),
                leftShift,
                symbol),
            new FunctionValue(leftShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value << int.CreateTruncating((T)b.Value))));
        var rightShift = symbol.Type.GetBinaryOperators(SyntaxKind.RightShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.BangToken),
                rightShift,
                symbol),
            new FunctionValue(rightShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value >> int.CreateTruncating((T)b.Value))));
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
                new LiteralValue(PredefinedTypes.Bool, ((T)a.Value).Equals(b.Value))));
        var notEquals = symbol.Type.GetBinaryOperators(SyntaxKind.NotEqualsOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.BangEqualsToken),
                notEquals,
                symbol),
            new FunctionValue(notEquals.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(PredefinedTypes.Bool, !((T)a.Value).Equals(b.Value))));
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
                new LiteralValue(PredefinedTypes.Bool, (T)a.Value < (T)b.Value)));
        var lessThanOrEqual = symbol.Type.GetBinaryOperators(SyntaxKind.LessThanOrEqualOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.LessThanEqualsToken),
                lessThanOrEqual,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(PredefinedTypes.Bool, (T)a.Value <= (T)b.Value)));
        var greaterThan = symbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.GreaterThanToken),
                greaterThan,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(PredefinedTypes.Bool, (T)a.Value > (T)b.Value)));
        var greaterThanOrEqual = symbol.Type.GetBinaryOperators(SyntaxKind.GreaterThanOrEqualOperator, symbol.Type, symbol.Type, PredefinedTypes.Bool).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.GreaterThanEqualsToken),
                greaterThanOrEqual,
                symbol),
            new FunctionValue(lessThan.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(PredefinedTypes.Bool, (T)a.Value >= (T)b.Value)));
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
                new LiteralValue(PredefinedTypes.Bool, !(bool)a.Value)));
        var logicalAnd = symbol.Type.GetBinaryOperators(SyntaxKind.LogicalAndOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.AmpersandAmpersandToken),
                logicalAnd,
                symbol),
            new FunctionValue(logicalAnd.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(PredefinedTypes.Bool, (bool)a.Value && (bool)b.Value)));
        var logicalOr = symbol.Type.GetBinaryOperators(SyntaxKind.LogicalOrOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.PipePipeToken),
                logicalOr,
                symbol),
            new FunctionValue(logicalOr.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(PredefinedTypes.Bool, (bool)a.Value || (bool)b.Value)));
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
