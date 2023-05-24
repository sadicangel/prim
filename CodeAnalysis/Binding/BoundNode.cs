namespace CodeAnalysis.Binding;

public abstract record class BoundNode(BoundNodeKind Kind)
{
    public abstract T Accept<T>(IBoundExpressionVisitor<T> visitor);
}
