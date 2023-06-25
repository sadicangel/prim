namespace CodeAnalysis.Syntax;

public sealed record class IfExpression(Token IfToken, Expression Condition, Expression Then, Token ElseToken, Expression Else) : Expression(NodeKind.IfExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return IfToken;
        yield return Condition;
        yield return Then;
        yield return ElseToken;
        yield return Else;
    }
}