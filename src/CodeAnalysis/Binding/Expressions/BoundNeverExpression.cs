using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundNeverExpression(SyntaxNode SyntaxNode)
    : BoundExpression(BoundKind.NeverExpression, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children() => [];
}
