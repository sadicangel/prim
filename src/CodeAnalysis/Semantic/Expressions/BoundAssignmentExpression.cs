using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundAssignmentExpression(SyntaxNode Syntax, BoundReference Left, BoundExpression Right)
    : BoundExpression(BoundKind.AssignmentExpression, Syntax, Left.Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
