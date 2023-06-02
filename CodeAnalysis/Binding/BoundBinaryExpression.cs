namespace CodeAnalysis.Binding;

internal sealed record class BoundBinaryExpression(BoundExpression Left, BoundBinaryOperator Operator, BoundExpression Right) : BoundExpression(BoundNodeKind.BinaryExpression, Operator.ResultType)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
}
