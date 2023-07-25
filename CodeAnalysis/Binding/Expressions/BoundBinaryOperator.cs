using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryOperator(TokenKind TokenKind, BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType, TypeSymbol ResultType)
{
    private static readonly Dictionary<TokenKind, List<BoundBinaryOperator>> Operators = CreateOperators();

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType)
        : this(tokenKind, kind, operandType, operandType, operandType)
    {
    }

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
        : this(tokenKind, kind, operandType, operandType, resultType)
    {

    }

    private static Dictionary<TokenKind, List<BoundBinaryOperator>> CreateOperators()
    {
        var operators = new Dictionary<TokenKind, List<BoundBinaryOperator>>
        {
            // cast operators
            [TokenKind.As] = new()
            {
                new BoundBinaryOperator(TokenKind.As, BoundBinaryOperatorKind.ExplicitCast, PredefinedTypes.Any, PredefinedTypes.Type, PredefinedTypes.Any)
            },

            // equality operators
            [TokenKind.EqualEqual] = new()
            {
                new BoundBinaryOperator(TokenKind.EqualEqual, BoundBinaryOperatorKind.Equal, PredefinedTypes.Any, PredefinedTypes.Bool)
            },
            [TokenKind.BangEqual] = new()
            {
                new BoundBinaryOperator(TokenKind.BangEqual, BoundBinaryOperatorKind.NotEqual, PredefinedTypes.Any, PredefinedTypes.Bool),
            }
        };

        var numberOperators = new TokenKind[] { TokenKind.Plus, TokenKind.Minus, TokenKind.Star, TokenKind.Slash, TokenKind.Percent, TokenKind.StarStar };
        var bitwiseOperators = new TokenKind[] { TokenKind.Ampersand, TokenKind.Pipe, TokenKind.Hat };
        var comparisonOperators = new TokenKind[] { TokenKind.Less, TokenKind.LessEqual, TokenKind.Greater, TokenKind.GreaterEqual };
        var logicalOperators = new TokenKind[] { TokenKind.AmpersandAmpersand, TokenKind.PipePipe };

        // integer operators
        var integers = PredefinedTypes.All.Where(t => t.IsInteger).ToList();
        operators.Add(numberOperators, integers.Select(i => (i, i, i)));
        operators.Add(bitwiseOperators, integers.Select(i => (i, i, i)));
        operators.Add(comparisonOperators, integers.Select(i => (i, i, PredefinedTypes.Bool)));

        // floating point operators
        var floats = PredefinedTypes.All.Where(t => t.IsFloatingPoint).ToList();
        operators.Add(numberOperators, floats.Select(f => (f, f, f)));
        operators.Add(comparisonOperators, floats.Select(f => (f, f, PredefinedTypes.Bool)));

        // bool operators
        var bools = new List<TypeSymbol> { PredefinedTypes.Bool };
        operators.Add(bitwiseOperators, bools.Select(b => (b, b, b)));
        operators.Add(comparisonOperators, bools.Select(b => (b, b, b)));
        operators.Add(logicalOperators, bools.Select(b => (b, b, b)));

        // str operators
        operators.Add(TokenKind.Plus, PredefinedTypes.Str, PredefinedTypes.Str, PredefinedTypes.Str);
        operators.Add(TokenKind.Plus, PredefinedTypes.Str, PredefinedTypes.Any, PredefinedTypes.Str);
        operators.Add(TokenKind.Plus, PredefinedTypes.Any, PredefinedTypes.Str, PredefinedTypes.Str);

        return operators;
    }

    private static BoundBinaryOperator[] Find(TokenKind kind, TypeSymbol left, TypeSymbol right, TypeSymbol? result)
    {
        if (!Operators.TryGetValue(kind, out var operators))
            return Array.Empty<BoundBinaryOperator>();

        return operators.Where(o => left.IsAssignableTo(o.LeftType) && right.IsAssignableTo(o.RightType) && (result?.IsAssignableTo(o.ResultType) ?? true)).ToArray();
    }

    public static BoundBinaryOperator? Bind(TokenKind tokenKind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol? resultType)
    {
        var matchingOperators = Find(tokenKind, leftType, rightType, resultType);

        var matchingOperator = matchingOperators.Length == 0 ? null : matchingOperators[0];

        if (matchingOperator?.Kind is BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast)
        {
            if (resultType is null)
                return null;

            var conversion = Conversion.Classify(leftType, resultType);
            if (!conversion.Exists)
                return null;

            //if(!conversion.IsIdentity)
            //Report unnecessary conversion.

            matchingOperator = matchingOperator with
            {
                LeftType = leftType,
                ResultType = resultType,
            };
        }

        return matchingOperator;
    }
}

file static class OperatorMapExtensions
{

    public static void Add(this Dictionary<TokenKind, List<BoundBinaryOperator>> map, IEnumerable<TokenKind> typeOperators, IEnumerable<(TypeSymbol left, TypeSymbol right, TypeSymbol result)> types)
    {
        foreach (var (left, right, result) in types)
            map.Add(typeOperators, left, right, result);
    }

    public static void Add(this Dictionary<TokenKind, List<BoundBinaryOperator>> map, IEnumerable<TokenKind> typeOperators, TypeSymbol left, TypeSymbol right, TypeSymbol result)
    {
        foreach (var @operator in typeOperators)
            map.Add(@operator, left, right, result);
    }

    public static void Add(this Dictionary<TokenKind, List<BoundBinaryOperator>> map, TokenKind @operator, TypeSymbol left, TypeSymbol right, TypeSymbol result)
    {
        if (!map!.TryGetValue(@operator, out var operators))
            map[@operator] = operators = new();
        operators.Add(new BoundBinaryOperator(@operator, @operator.GetBinaryOperatorKind(), left, right, result));
    }
}