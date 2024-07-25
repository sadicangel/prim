using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundInvocationExpression LowerInvocationExpression(BoundInvocationExpression node)
    {
        var expression = LowerExpression(node.Expression);
        if (ReferenceEquals(expression, node.Expression))
            return node;

        return node with { Expression = expression };
    }
}
