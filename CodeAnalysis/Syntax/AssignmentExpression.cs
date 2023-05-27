namespace CodeAnalysis.Syntax;

public sealed record class AssignmentExpression(Token IdentifierToken, Token EqualsToken, Expression Expression) : Expression(NodeKind.AssignmentExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<IPrintableNode> GetChildren()
    {
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
    }
}
