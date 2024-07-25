using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundBinaryExpression LowerBinaryExpression(BoundBinaryExpression node)
    {
        var left = LowerExpression(node.Left);
        var right = LowerExpression(node.Right);

        if (ReferenceEquals(left, node.Left) && ReferenceEquals(right, node.Right))
            return node;

        return node with { Left = left, Right = right };
    }
}
