using CodeAnalysis.Syntax;
using System.Linq.Expressions;

namespace CodeAnalysis.Binding.Expressions;

internal enum BoundBinaryOperatorKind
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Exponent,
    And,
    Or,
    ExclusiveOr,
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LeftShift,
    RightShift,
    AndAlso,
    OrElse,
    ExplicitCast,
    ImplicitCast,
}

internal static class BoundBinaryOperatorKindExtensions
{
    public static string GetLinqExpressionName(this BoundBinaryOperatorKind kind) => kind switch
    {
        BoundBinaryOperatorKind.ExplicitCast or BoundBinaryOperatorKind.ImplicitCast => nameof(Expression.Convert),
        _ => kind.ToString(),
    };

    public static BoundBinaryOperatorKind GetBinaryOperatorKind(this TokenKind kind)
    {
        switch (kind)
        {
            case TokenKind.Ampersand:
                return BoundBinaryOperatorKind.And;
            case TokenKind.AmpersandAmpersand:
                return BoundBinaryOperatorKind.AndAlso;
            case TokenKind.BangEqual:
                return BoundBinaryOperatorKind.NotEqual;
            case TokenKind.EqualEqual:
                return BoundBinaryOperatorKind.Equal;
            case TokenKind.Greater:
                return BoundBinaryOperatorKind.GreaterThan;
            case TokenKind.GreaterEqual:
                return BoundBinaryOperatorKind.GreaterThanOrEqual;
            case TokenKind.Hat:
                return BoundBinaryOperatorKind.ExclusiveOr;
            case TokenKind.Less:
                return BoundBinaryOperatorKind.LessThan;
            case TokenKind.LessEqual:
                return BoundBinaryOperatorKind.LessThanOrEqual;
            case TokenKind.Minus:
                return BoundBinaryOperatorKind.Subtract;
            case TokenKind.Percent:
                return BoundBinaryOperatorKind.Modulo;
            case TokenKind.Pipe:
                return BoundBinaryOperatorKind.Or;
            case TokenKind.PipePipe:
                return BoundBinaryOperatorKind.OrElse;
            case TokenKind.Plus:
                return BoundBinaryOperatorKind.Add;
            case TokenKind.Slash:
                return BoundBinaryOperatorKind.Divide;
            case TokenKind.Star:
                return BoundBinaryOperatorKind.Multiply;
            case TokenKind.StarStar:
                return BoundBinaryOperatorKind.Exponent;
            default:
                throw new InvalidOperationException($"Token {kind} is not a binary operator");
        }
    }
}