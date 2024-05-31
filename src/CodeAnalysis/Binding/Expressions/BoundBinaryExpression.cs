using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBinaryExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    BoundExpression Left,
    object Operator,
    BoundExpression Right)
    : BoundExpression(BoundKind, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return (BoundNode)Operator;
        yield return Right;
    }
}
