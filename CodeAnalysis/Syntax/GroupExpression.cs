namespace CodeAnalysis.Syntax;

public sealed record class GroupExpression(Token OpenParenthesisToken, Expression Expression, Token CloseParenthesisToken) : Expression(SyntaxNodeKind.GroupExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OpenParenthesisToken;
        yield return Expression;
        yield return CloseParenthesisToken;
    }
}
