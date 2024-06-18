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
        var unaryMinus = symbol.Type.GetBinaryOperators(SyntaxKind.UnaryMinusOperator, symbol.Type, symbol.Type, symbol.Type).Single();
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

    public static StructValue AddBitwiseOperators<T>(this StructValue s, StructSymbol symbol) where T : IBinaryInteger<T>, IShiftOperators<T, T, T>
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
                new LiteralValue(a.Type, (T)a.Value << (T)b.Value)));
        var rightShift = symbol.Type.GetBinaryOperators(SyntaxKind.RightShiftOperator, symbol.Type, symbol.Type, symbol.Type).Single();
        s.SetOperator(
            new OperatorSymbol(
                SyntaxFactory.SyntheticToken(SyntaxKind.BangToken),
                rightShift,
                symbol),
            new FunctionValue(rightShift.Type, (PrimValue a, PrimValue b) =>
                new LiteralValue(a.Type, (T)a.Value >> (T)b.Value)));
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
        var not = symbol.Type.GetBinaryOperators(SyntaxKind.NotOperator, symbol.Type, symbol.Type, symbol.Type).Single();
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
