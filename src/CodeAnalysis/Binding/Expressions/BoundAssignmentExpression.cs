using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundAssignmentExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    PrimType Type,
    BoundExpression Left,
    BoundExpression Right)
    : BoundExpression(BoundKind, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
