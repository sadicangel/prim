namespace CodeAnalysis.Syntax;

public sealed record class BinaryExpression(Expression Left, Token OperatorToken, Expression Right) : Expression(NodeKind.BinaryExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<INode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}