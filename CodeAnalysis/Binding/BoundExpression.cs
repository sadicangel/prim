namespace CodeAnalysis.Binding;

internal abstract record class BoundExpression(BoundNodeKind Kind, Type Type) : BoundNode(Kind)
{
    public abstract T Accept<T>(IBoundExpressionVisitor<T> visitor);
}