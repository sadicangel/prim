using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

public sealed record class BoundBinaryOperator(TokenKind TokenKind, BoundBinaryOperatorKind Kind, Type LeftType, Type RightType, Type ResultType)
{
    private static readonly BoundBinaryOperator[] Operators =
    {
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Addition, typeof(long)),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtraction, typeof(long)),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiplication, typeof(long)),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Division, typeof(long)),

        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equals, typeof(long), typeof(bool)),
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equals, typeof(bool)),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(long), typeof(bool)),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool)),

        new BoundBinaryOperator(TokenKind.AmpersandAmpersand, BoundBinaryOperatorKind.LogicalAnd, typeof(bool)),
        new BoundBinaryOperator(TokenKind.PipePipe, BoundBinaryOperatorKind.LogicalOr, typeof(bool)),
    };

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, Type operandType)
        : this(tokenKind, kind, operandType, operandType, operandType)
    {

    }

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, Type operandType, Type resultType)
        : this(tokenKind, kind, operandType, operandType, resultType)
    {

    }

    public static BoundBinaryOperator? Bind(TokenKind tokenKind, Type leftType, Type rightType)
    {
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind && @operator.LeftType == leftType && @operator.RightType == rightType)
                return @operator;
        }

        return null;
    }
}