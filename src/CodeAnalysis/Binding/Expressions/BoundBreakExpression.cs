using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBreakExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Result
)
    : BoundExpression(BoundNodeKind.BreakExpression, SyntaxNode, Result.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Result;
    }
}

