using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundAssignmentExpression(
    SyntaxNode Syntax,
    PrimType Type,
    BoundExpression Left,
    BoundExpression Right)
    : BoundExpression(BoundKind.AssignmentExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
