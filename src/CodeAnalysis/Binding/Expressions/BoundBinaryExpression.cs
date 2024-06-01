using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBinaryExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    PrimType Type,
    BoundExpression Left,
    object Operator,
    BoundExpression Right)
    : BoundExpression(BoundKind, SyntaxNode, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return (BoundNode)Operator;
        yield return Right;
    }
}
