using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundBinaryOperator(TokenKind TokenKind, BoundBinaryOperatorKind Kind, Type LeftType, Type RightType, Type ResultType) : BoundOperator
{
    private static readonly BoundBinaryOperator[] Operators =
    {
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Addition, typeof(int)),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtraction, typeof(int)),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiplication, typeof(int)),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Division, typeof(int)),
        new BoundBinaryOperator(TokenKind.Percent, BoundBinaryOperatorKind.Modulo, typeof(int)),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, typeof(int)),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, typeof(int)),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, typeof(int)),

        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equals, typeof(int), typeof(bool)),
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equals, typeof(bool)),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEquals, typeof(bool)),

        new BoundBinaryOperator(TokenKind.Less, BoundBinaryOperatorKind.LessThan, typeof(int), typeof(bool)),
        new BoundBinaryOperator(TokenKind.LessEquals, BoundBinaryOperatorKind.LessThanOrEqualTo, typeof(int), typeof(bool)),
        new BoundBinaryOperator(TokenKind.Greater, BoundBinaryOperatorKind.GreaterThan, typeof(int), typeof(bool)),
        new BoundBinaryOperator(TokenKind.GreaterEquals, BoundBinaryOperatorKind.GreaterThanOrEqualTo, typeof(int), typeof(bool)),

        new BoundBinaryOperator(TokenKind.AmpersandAmpersand, BoundBinaryOperatorKind.AndAlso, typeof(bool)),
        new BoundBinaryOperator(TokenKind.PipePipe, BoundBinaryOperatorKind.OrElse, typeof(bool)),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, typeof(bool)),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, typeof(bool)),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, typeof(bool)),
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

    protected override string GetDisplayString() => $"{ResultType} operator{TokenKind.GetText()}({LeftType}, {RightType})";
}