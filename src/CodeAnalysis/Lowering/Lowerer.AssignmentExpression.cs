using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundAssignmentExpression LowerAssignmentExpression(BoundAssignmentExpression node, Context context)
    {
        var left = LowerExpression(node.Left, context);
        var right = LowerExpression(node.Right, context);
        if (ReferenceEquals(left, node.Left) && ReferenceEquals(right, node.Right))
            return node;

        return node with { Left = left, Right = right };
    }
}
