namespace CodeAnalysis.Syntax;

public sealed record class NameExpression(Token IdentifierToken) : Expression(NodeKind.NameExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<IPrintableNode> GetChildren()
    {
        yield return IdentifierToken;
    }
}