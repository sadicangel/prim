using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundAssignmentExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    BoundExpression Left,
    BoundExpression Right)
    : BoundExpression(BoundKind, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
