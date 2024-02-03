using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundContinueExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Result
)
    : BoundExpression(BoundNodeKind.ContinueExpression, SyntaxNode, Result.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Result;
    }
}

