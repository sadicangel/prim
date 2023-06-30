using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal abstract record class BoundExpression(BoundNodeKind NodeKind, TypeSymbol Type) : BoundNode(NodeKind)
{
    public abstract T Accept<T>(IBoundExpressionVisitor<T> visitor);
}