using CodeAnalysis.Operators;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBinaryExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Left,
    BinaryOperator Operator,
    BoundExpression Right
)
    : BoundExpression(BoundNodeKind.BinaryExpression, SyntaxNode, Operator.ResultType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
