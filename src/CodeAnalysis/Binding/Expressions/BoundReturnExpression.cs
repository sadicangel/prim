using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundReturnExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Result
)
    : BoundExpression(BoundNodeKind.ReturnExpression, SyntaxNode, Result.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Result;
    }
}

