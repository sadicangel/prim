using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal abstract record class BoundExpression(BoundNodeKind Kind, TypeSymbol Type) : BoundNode(Kind)
{
    public abstract T Accept<T>(IBoundExpressionVisitor<T> visitor);
}