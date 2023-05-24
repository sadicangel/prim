using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

public sealed record class BoundUnaryOperator(TokenKind TokenKind, BoundUnaryOperatorKind Kind, Type OperandType, Type ResultType)
{
    private static readonly BoundUnaryOperator[] Operators =
    {
        new BoundUnaryOperator(TokenKind.Bang, BoundUnaryOperatorKind.LogicalNegation, typeof(bool)),
        new BoundUnaryOperator(TokenKind.Plus, BoundUnaryOperatorKind.Identity, typeof(long)),
        new BoundUnaryOperator(TokenKind.Minus, BoundUnaryOperatorKind.Negation, typeof(long)),
    };

    public BoundUnaryOperator(TokenKind tokenKind, BoundUnaryOperatorKind kind, Type operandType)
        : this(tokenKind, kind, operandType, operandType)
    {

    }

    public static BoundUnaryOperator? Bind(TokenKind tokenKind, Type operandType)
    {
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind && @operator.OperandType == operandType)
                return @operator;
        }

        return null;
    }
}