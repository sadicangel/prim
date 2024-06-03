using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundAssignmentExpression(
    SyntaxNode SyntaxNode,
    PrimType Type,
    BoundExpression Left,
    BoundExpression Right)
    : BoundExpression(BoundKind.AssignmentExpression, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
