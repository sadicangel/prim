using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundBinaryOperator(TokenKind TokenKind, BoundBinaryOperatorKind Kind, TypeSymbol LeftType, TypeSymbol RightType, TypeSymbol ResultType)
{
    private static readonly BoundBinaryOperator[] Operators =
    {
        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Addition, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Minus, BoundBinaryOperatorKind.Subtraction, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Star, BoundBinaryOperatorKind.Multiplication, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Slash, BoundBinaryOperatorKind.Division, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Percent, BoundBinaryOperatorKind.Modulo, TypeSymbol.I32),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, TypeSymbol.I32),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, TypeSymbol.I32),

        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equals, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.EqualsEquals, BoundBinaryOperatorKind.Equals, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.BangEquals, BoundBinaryOperatorKind.NotEquals, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Less, BoundBinaryOperatorKind.LessThan, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.LessEquals, BoundBinaryOperatorKind.LessThanOrEqualTo, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Greater, BoundBinaryOperatorKind.GreaterThan, TypeSymbol.I32, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.GreaterEquals, BoundBinaryOperatorKind.GreaterThanOrEqualTo, TypeSymbol.I32, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.AmpersandAmpersand, BoundBinaryOperatorKind.AndAlso, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.PipePipe, BoundBinaryOperatorKind.OrElse, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Ampersand, BoundBinaryOperatorKind.And, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Pipe, BoundBinaryOperatorKind.Or, TypeSymbol.Bool),
        new BoundBinaryOperator(TokenKind.Hat, BoundBinaryOperatorKind.ExclusiveOr, TypeSymbol.Bool),

        new BoundBinaryOperator(TokenKind.Plus, BoundBinaryOperatorKind.Addition, TypeSymbol.String),
    };

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType)
        : this(tokenKind, kind, operandType, operandType, operandType)
    {
    }

    public BoundBinaryOperator(TokenKind tokenKind, BoundBinaryOperatorKind kind, TypeSymbol operandType, TypeSymbol resultType)
        : this(tokenKind, kind, operandType, operandType, resultType)
    {

    }

    public static BoundBinaryOperator? Bind(TokenKind tokenKind, TypeSymbol leftType, TypeSymbol rightType)
    {
        foreach (var @operator in Operators)
        {
            if (@operator.TokenKind == tokenKind && @operator.LeftType == leftType && @operator.RightType == rightType)
                return @operator;
        }

        return null;
    }
}