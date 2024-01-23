using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal enum BoundUnaryOperatorKind
{
    UnaryPlus,
    Negate,
    Increment,
    Decrement,
    Not,
    OnesComplement
}

internal static class BoundUnaryOperatorKindExtensions
{
    public static string GetLinqExpressionName(this BoundUnaryOperatorKind kind) => kind switch
    {
        _ => kind.ToString()
    };

    public static BoundUnaryOperatorKind GetUnaryOperatorKind(this TokenKind kind)
    {
        switch (kind)
        {
            case TokenKind.Bang:
                return BoundUnaryOperatorKind.Not;
            case TokenKind.Minus:
                return BoundUnaryOperatorKind.Negate;
            case TokenKind.Plus:
                return BoundUnaryOperatorKind.UnaryPlus;
            case TokenKind.Tilde:
                return BoundUnaryOperatorKind.OnesComplement;
            default:
                throw new InvalidOperationException($"Token {kind} is not a unary operator");
        }
    }
}