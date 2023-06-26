namespace CodeAnalysis.Syntax;

public sealed record class NameExpression(Token Identifier) : Expression(SyntaxNodeKind.NameExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
    }
}