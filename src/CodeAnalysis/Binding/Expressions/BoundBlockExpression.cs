using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBlockExpression(
    SyntaxNode SyntaxNode,
    IReadOnlyList<BoundExpression> Expressions
)
    : BoundExpression(BoundNodeKind.BlockExpression, SyntaxNode, Expressions.LastOrDefault()?.Type ?? PredefinedTypes.Unit)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var expression in Expressions)
            yield return expression;
    }
}
