namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(BoundUnaryOperator Operator, BoundExpression Operand) : BoundExpression(BoundNodeKind.UnaryExpression, Operator.ResultType)
{
    public override ConstantValue? ConstantValue { get; } = ConstantFolding.Compute(Operator, Operand);
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Operand;
    }

    public override string ToString() => base.ToString();
}
