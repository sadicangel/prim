namespace CodeAnalysis.Syntax;

public sealed record class ConvertExpression(Expression Expression, Token AsToken, Token TypeToken) : Expression(NodeKind.ConvertExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return Expression;
        yield return AsToken;
        yield return TypeToken;
    }
}
