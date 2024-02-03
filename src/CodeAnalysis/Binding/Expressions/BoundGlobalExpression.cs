using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundGlobalExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Declaration
)
    : BoundExpression(BoundNodeKind.GlobalExpression, SyntaxNode, Declaration.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Declaration;
    }
}
