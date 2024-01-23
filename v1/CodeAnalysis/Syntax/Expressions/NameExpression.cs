namespace CodeAnalysis.Syntax.Expressions;

public sealed record class NameExpression(SyntaxTree SyntaxTree, Token Identifier)
    : Expression(SyntaxNodeKind.NameExpression, SyntaxTree)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
    }
    public override string ToString() => base.ToString();
}