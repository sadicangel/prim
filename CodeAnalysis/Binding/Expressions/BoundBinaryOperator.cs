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

    public static IReadOnlyList<BoundBinaryOperator> Bind(TokenKind tokenKind, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol? resultType)
    {
        if (!Operators.TryGetValue(tokenKind, out var operators))
            return Array.Empty<BoundBinaryOperator>();

        if (resultType is null || operators[0].Kind is not BoundBinaryOperatorKind.ExplicitCast and not BoundBinaryOperatorKind.ImplicitCast)
        {
            var matchingOperators = operators.Where(TypesAreAssignable);
            // If an operator has types that match exactly, pick that one.
            if (matchingOperators.Take(2).Any())
            {
                var exactMatchOperators = matchingOperators.Where(o => leftType == o.LeftType && rightType == o.RightType);
                if (exactMatchOperators.Any())
                    matchingOperators = exactMatchOperators;
            }
            return matchingOperators.ToArray();
        }

        // Return modified convert operators. Maybe cache this?
        var convertOperators = new List<BoundBinaryOperator>();
        foreach (var @operator in operators)
        {
            if (TypesAreAssignable(@operator))
            {
                var conversion = Conversion.Classify(leftType, resultType);
                if (conversion.Exists)
                {
                    convertOperators.Add(@operator with
                    {
                        LeftType = leftType,
                        ResultType = resultType,
                    });
                }
            }
        }
        return convertOperators.ToArray();

        bool TypesAreAssignable(BoundBinaryOperator o) => leftType.IsAssignableTo(o.LeftType) && rightType.IsAssignableTo(o.RightType) && (resultType?.IsAssignableTo(o.ResultType) ?? true);
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
        var integerCombinations = OperatorType.GetIntegerCombinations().ToList();
        // Pick all possible combinations: current integer <op> other integer => widest integer between the two
        operators.Add(numberOperators, integerCombinations);
        operators.Add(bitwiseOperators, integerCombinations);
        operators.Add(comparisonOperators, integerCombinations.Select(i => i with { Result = PredefinedTypes.Bool }));

        // floating point operators
        var floatCombinations = OperatorType.GetFloatCombinations().ToList();
        operators.Add(numberOperators, floatCombinations);
        operators.Add(comparisonOperators, floatCombinations.Select(f => f with { Result = PredefinedTypes.Bool }));

        // bool operators
        var bools = new List<TypeSymbol> { PredefinedTypes.Bool };
        operators.Add(bitwiseOperators, bools.Select(b => new OperatorType(b, b, b)));
        operators.Add(comparisonOperators, bools.Select(b => new OperatorType(b, b, b)));
        operators.Add(logicalOperators, bools.Select(b => new OperatorType(b, b, b)));

        // str operators
        operators.Add(TokenKind.Plus, PredefinedTypes.Str, PredefinedTypes.Str, PredefinedTypes.Str);
        operators.Add(TokenKind.Plus, PredefinedTypes.Str, PredefinedTypes.Any, PredefinedTypes.Str);
        operators.Add(TokenKind.Plus, PredefinedTypes.Any, PredefinedTypes.Str, PredefinedTypes.Str);

        return operators;
    }
}

file readonly record struct OperatorType(TypeSymbol Left, TypeSymbol Right, TypeSymbol Result)
{
    public static IEnumerable<OperatorType> GetIntegerCombinations()
    {
        var integers = PredefinedTypes.All.Where(t => t.IsInteger).ToArray();

        for (int i = 0; i < integers.Length; ++i)
        {
            var t1 = integers[i];
            yield return new OperatorType(t1, t1, t1);
            for (int j = i + 1; j < integers.Length; ++j)
            {
                var t2 = integers[j];
                yield return new OperatorType(t1, t2, GetLargestType(t1, t2));
                yield return new OperatorType(t2, t1, GetLargestType(t2, t1));
            }
        }

        static TypeSymbol GetLargestType(TypeSymbol t1, TypeSymbol t2)
        {
            // Both signed or both unsigned or whatever is largest.
            if (t1.IsSignedInteger == t2.IsSignedInteger)
                return t1.BinarySize >= t2.BinarySize ? t1 : t2;

            // If both are same size and different signs, pick the next larger signed
            // integer after t1, if any; otherwise select <never>. This allows ambiguous matches.
            return (t1.Name, t2.Name) switch
            {
                (PredefinedTypeNames.I8, PredefinedTypeNames.U8) => PredefinedTypes.I16,
                (PredefinedTypeNames.I8, PredefinedTypeNames.U16) => PredefinedTypes.I32,
                (PredefinedTypeNames.I8, PredefinedTypeNames.U32) => PredefinedTypes.I64,
                (PredefinedTypeNames.I16, PredefinedTypeNames.U8) => PredefinedTypes.I16,
                (PredefinedTypeNames.I16, PredefinedTypeNames.U16) => PredefinedTypes.I32,
                (PredefinedTypeNames.I16, PredefinedTypeNames.U32) => PredefinedTypes.I64,
                (PredefinedTypeNames.I32, PredefinedTypeNames.U8) => PredefinedTypes.I32,
                (PredefinedTypeNames.I32, PredefinedTypeNames.U16) => PredefinedTypes.I32,
                (PredefinedTypeNames.I32, PredefinedTypeNames.U32) => PredefinedTypes.I64,
                (PredefinedTypeNames.I64, PredefinedTypeNames.U8) => PredefinedTypes.I64,
                (PredefinedTypeNames.I64, PredefinedTypeNames.U16) => PredefinedTypes.I64,
                (PredefinedTypeNames.I64, PredefinedTypeNames.U32) => PredefinedTypes.I64,
                (PredefinedTypeNames.ISize, PredefinedTypeNames.U8) => PredefinedTypes.ISize,
                (PredefinedTypeNames.ISize, PredefinedTypeNames.U16) => PredefinedTypes.ISize,
                (PredefinedTypeNames.ISize, PredefinedTypeNames.U32) when IntPtr.Size == 8 => PredefinedTypes.ISize,

                (PredefinedTypeNames.U8, PredefinedTypeNames.I8) => PredefinedTypes.I16,
                (PredefinedTypeNames.U8, PredefinedTypeNames.I16) => PredefinedTypes.I32,
                (PredefinedTypeNames.U8, PredefinedTypeNames.I32) => PredefinedTypes.I32,
                (PredefinedTypeNames.U8, PredefinedTypeNames.I64) => PredefinedTypes.I64,
                (PredefinedTypeNames.U8, PredefinedTypeNames.ISize) => PredefinedTypes.ISize,
                (PredefinedTypeNames.U16, PredefinedTypeNames.I8) => PredefinedTypes.I32,
                (PredefinedTypeNames.U16, PredefinedTypeNames.I16) => PredefinedTypes.I32,
                (PredefinedTypeNames.U16, PredefinedTypeNames.I32) => PredefinedTypes.I64,
                (PredefinedTypeNames.U16, PredefinedTypeNames.I64) => PredefinedTypes.I64,
                (PredefinedTypeNames.U16, PredefinedTypeNames.ISize) => PredefinedTypes.I64,
                (PredefinedTypeNames.U32, PredefinedTypeNames.I8) => PredefinedTypes.I64,
                (PredefinedTypeNames.U32, PredefinedTypeNames.I16) => PredefinedTypes.I64,
                (PredefinedTypeNames.U32, PredefinedTypeNames.I32) => PredefinedTypes.I64,
                (PredefinedTypeNames.U32, PredefinedTypeNames.I64) => PredefinedTypes.I64,
                (PredefinedTypeNames.U32, PredefinedTypeNames.ISize) when IntPtr.Size == 8 => PredefinedTypes.ISize,

                _ => PredefinedTypes.Never,
            };
        }
    }
    public static IEnumerable<OperatorType> GetFloatCombinations()
    {
        var floats = PredefinedTypes.All.Where(t => t.IsFloatingPoint).ToArray();

        for (int i = 0; i < floats.Length; ++i)
        {
            var t1 = floats[i];
            yield return new OperatorType(t1, t1, t1);
            for (int j = i + 1; j < floats.Length; ++j)
            {
                var t2 = floats[j];
                yield return new OperatorType(t1, t2, t1.BinarySize >= t2.BinarySize ? t1 : t2);
                yield return new OperatorType(t2, t1, t2.BinarySize >= t1.BinarySize ? t2 : t1);
            }
        }

        var integers = PredefinedTypes.All.Where(t => t.IsInteger).ToArray();
        foreach (var t1 in floats)
        {
            for (int i = 0; i < integers.Length; ++i)
            {
                var t2 = integers[i];
                yield return new OperatorType(t1, t2, t1);
                yield return new OperatorType(t2, t1, t1);
            }
        }
    }
}

file static class OperatorMapExtensions
{
    public static void Add(this Dictionary<TokenKind, List<BoundBinaryOperator>> map, IEnumerable<TokenKind> typeOperators, IEnumerable<OperatorType> infos)
    {
        foreach (var info in infos)
            map.Add(typeOperators, info);
    }

    public static void Add(this Dictionary<TokenKind, List<BoundBinaryOperator>> map, IEnumerable<TokenKind> typeOperators, OperatorType info)
    {
        var (left, right, result) = info;
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