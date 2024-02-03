using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNeverExpression(
    SyntaxNode SyntaxNode
)
    : BoundExpression(BoundNodeKind.NeverExpression, SyntaxNode, PredefinedTypes.Never)
{
    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
