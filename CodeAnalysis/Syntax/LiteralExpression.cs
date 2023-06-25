namespace CodeAnalysis.Syntax;

public sealed record class LiteralExpression(Token LiteralToken, object? Value) : Expression(SyntaxNodeKind.LiteralExpression)
{
    public LiteralExpression(Token LiteralToken) : this(LiteralToken, LiteralToken.Value) { }

    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<SyntaxNode> GetChildren() { yield return LiteralToken; }
}
