using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryOperator(TokenKind TokenKind, BoundUnaryOperatorKind Kind, TypeSymbol OperandType, TypeSymbol ResultType)
{
    private static readonly Dictionary<TokenKind, List<BoundUnaryOperator>> Operators = CreateOperators();

    public BoundUnaryOperator(TokenKind tokenKind, BoundUnaryOperatorKind kind, TypeSymbol operandType)
        : this(tokenKind, kind, operandType, operandType)
    {
    }

    public static IReadOnlyList<BoundUnaryOperator> Bind(TokenKind tokenKind, TypeSymbol operandType)
    {
        if (!Operators.TryGetValue(tokenKind, out var operators))
            return Array.Empty<BoundUnaryOperator>();

        return operators.Where(o => operandType.IsAssignableTo(o.OperandType)).ToArray();
    }

    private static Dictionary<TokenKind, List<BoundUnaryOperator>> CreateOperators()
    {
        var operators = new Dictionary<TokenKind, List<BoundUnaryOperator>>
        {
            [TokenKind.Bang] = new()
            {
                new BoundUnaryOperator(TokenKind.Bang, BoundUnaryOperatorKind.Not, PredefinedTypes.Bool)
            }
        };

        var numberOperators = new TokenKind[] { TokenKind.Plus, TokenKind.Minus };
        var bitwiseOperators = new TokenKind[] { TokenKind.Tilde };

        // integer operators
        var integers = PredefinedTypes.All.Where(t => t.IsInteger).ToList();
        operators.Add(numberOperators, integers.Select(t => new OperatorType(t, t)));
        operators.Add(bitwiseOperators, integers.Select(t => new OperatorType(t, t)));

        // floating point operators
        var floats = PredefinedTypes.All.Where(t => t.IsFloatingPoint).ToList();
        operators.Add(numberOperators, floats.Select(t => new OperatorType(t, t)));

        return operators;
    }
}

file readonly record struct OperatorType(TypeSymbol Operand, TypeSymbol Result);

file static class OperatorMapExtensions
{
    public static void Add(this Dictionary<TokenKind, List<BoundUnaryOperator>> map, IEnumerable<TokenKind> typeOperators, IEnumerable<OperatorType> infos)
    {
        foreach (var info in infos)
            map.Add(typeOperators, info);
    }

    public static void Add(this Dictionary<TokenKind, List<BoundUnaryOperator>> map, IEnumerable<TokenKind> typeOperators, OperatorType info)
    {
        var (operand, result) = info;
        foreach (var @operator in typeOperators)
            map.Add(@operator, operand, result);
    }

    public static void Add(this Dictionary<TokenKind, List<BoundUnaryOperator>> map, TokenKind @operator, TypeSymbol operand, TypeSymbol result)
    {
        if (!map!.TryGetValue(@operator, out var operators))
            map[@operator] = operators = new();
        operators.Add(new BoundUnaryOperator(@operator, @operator.GetUnaryOperatorKind(), operand, result));
    }
}