namespace CodeAnalysis.Syntax;

public sealed record class BinaryExpression(SyntaxTree SyntaxTree, Expression Left, Token Operator, Expression Right)
    : Expression(SyntaxNodeKind.BinaryExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}