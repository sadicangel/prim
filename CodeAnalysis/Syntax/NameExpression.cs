namespace CodeAnalysis.Syntax;

public sealed record class NameExpression(Token IdentifierToken) : Expression(SyntaxNodeKind.NameExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return IdentifierToken;
    }
}