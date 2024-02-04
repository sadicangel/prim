using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundArgumentListExpression(
    SyntaxNode SyntaxNode,
    IReadOnlyList<BoundExpression> Arguments
)
    : BoundExpression(BoundNodeKind.ArgumentList, SyntaxNode, new TypeList([.. Arguments.Select(a => a.Type)]))
{
    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
