using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNeverExpression(SyntaxNode SyntaxNode)
    : BoundExpression(BoundKind.NeverExpression, SyntaxNode, PredefinedTypes.Never)
{
    public override IEnumerable<BoundNode> Children() => [];
}
