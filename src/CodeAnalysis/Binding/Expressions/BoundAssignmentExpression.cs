using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundAssignmentExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
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
