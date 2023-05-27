namespace CodeAnalysis.Binding;

public sealed record class BoundAssignmentExpression(Variable Variable, BoundExpression Expression) : BoundExpression(BoundNodeKind.AssignmentExpression, Expression.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
}