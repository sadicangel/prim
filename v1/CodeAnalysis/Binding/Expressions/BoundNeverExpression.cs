using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundNeverExpression(SyntaxNode Syntax)
    : BoundExpression(BoundNodeKind.NeverExpression, Syntax, PredefinedTypes.Never)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children() => Enumerable.Empty<INode>();
}