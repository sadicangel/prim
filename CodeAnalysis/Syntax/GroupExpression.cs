namespace CodeAnalysis.Syntax;

public sealed record class GroupExpression(Token OpenParenthesis, Expression Expression, Token CloseParenthesis) : Expression(SyntaxNodeKind.GroupExpression)
{
    public override T Accept<T>(ISyntaxExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return OpenParenthesis;
        yield return Expression;
        yield return CloseParenthesis;
    }
}
