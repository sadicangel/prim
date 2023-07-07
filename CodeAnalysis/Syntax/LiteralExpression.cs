namespace CodeAnalysis.Syntax;

public sealed record class LiteralExpression(SyntaxTree SyntaxTree, Token LiteralToken, object? Value)
    : Expression(SyntaxNodeKind.LiteralExpression, SyntaxTree)
{
    public LiteralExpression(SyntaxTree syntaxTree, Token literal) : this(syntaxTree, literal, literal.Value) { }

    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);

    public override IEnumerable<SyntaxNode> GetChildren() { yield return LiteralToken; }
}
