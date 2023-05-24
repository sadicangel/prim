namespace CodeAnalysis.Syntax;

public sealed record class LiteralExpression(Token LiteralToken, object? Value) : Expression(NodeKind.LiteralExpression)
{
    public LiteralExpression(Token LiteralToken) : this(LiteralToken, LiteralToken.Value) { }

    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<IPrintableNode> GetChildren() { yield return LiteralToken; }
}
