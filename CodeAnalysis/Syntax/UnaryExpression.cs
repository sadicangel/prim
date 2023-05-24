namespace CodeAnalysis.Syntax;

public sealed record class UnaryExpression(Token OperatorToken, Expression Operand) : Expression(NodeKind.UnaryExpression)
{
    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<IPrintableNode> GetChildren()
    {
        yield return OperatorToken;
        yield return Operand;
    }
}
