namespace CodeAnalysis.Syntax;

public sealed record class BinaryExpression(Expression Left, Token Operator, Expression Right) : Expression(SyntaxNodeKind.BinaryExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}