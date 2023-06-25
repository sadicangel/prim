namespace CodeAnalysis.Syntax;

public sealed record class UnaryExpression(Token OperatorToken, Expression Operand) : Expression(NodeKind.UnaryExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<Node> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}
