namespace CodeAnalysis.Binding;

internal sealed record class BoundUnaryExpression(BoundUnaryOperator Operator, BoundExpression Operand) : BoundExpression(BoundNodeKind.UnaryExpression, Operator.ResultType)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Operand;
    }
}
