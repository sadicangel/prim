namespace CodeAnalysis.Syntax;

public sealed record class GroupExpression(Token OpenParenthesisToken, Expression Expression, Token CloseParenthesisToken) : Expression(NodeKind.GroupExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<IPrintableNode> GetChildren()
    {
        yield return OpenParenthesisToken;
        yield return Expression;
        yield return CloseParenthesisToken;
    }
}
