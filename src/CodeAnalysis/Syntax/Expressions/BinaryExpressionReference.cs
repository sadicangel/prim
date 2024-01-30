namespace CodeAnalysis.Syntax.Expressions;

public record class BinaryExpressionReference(
    SyntaxTree SyntaxTree,
    Expression Left,
    Token Dot,
    Expression Right
)
    : BinaryExpression(SyntaxTree, Left, Dot, Right);
