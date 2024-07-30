using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundStackInstantiationExpression LowerStackInstantiationExpression(BoundStackInstantiationExpression node, Context context)
    {
        var expression = LowerExpression(node.Expression, context);
        if (ReferenceEquals(expression, node.Expression))
            return node;

        return node with { Expression = expression };
    }
}
