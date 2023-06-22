using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundNeverExpression() : BoundExpression(BoundNodeKind.ErrorExpression, TypeSymbol.Never)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();
}