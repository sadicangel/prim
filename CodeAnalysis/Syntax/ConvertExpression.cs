namespace CodeAnalysis.Syntax;

public sealed record class ConvertExpression(Expression Expression, Token AsToken, Token TypeToken) : Expression(SyntaxNodeKind.ConvertExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
        yield return AsToken;
        yield return TypeToken;
    }
}
