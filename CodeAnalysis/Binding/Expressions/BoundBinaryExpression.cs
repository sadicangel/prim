namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(BoundExpression Left, BoundBinaryOperator Operator, BoundExpression Right) : BoundExpression(BoundNodeKind.BinaryExpression, Operator.ResultType)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Left;
        yield return Right;
    }
    public override string ToString() => base.ToString();
}
