using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundStructInitExpression LowerStructInitExpression(BoundStructInitExpression node, LowererContext context)
    {
        var properties = LowerList(node.Properties, context, LowerPropertyInitExpression);
        if (properties is null)
            return node;

        return node with { Properties = new(properties) };

        static BoundPropertyInitExpression LowerPropertyInitExpression(BoundPropertyInitExpression node, LowererContext context)
        {
            var expression = LowerExpression(node.Expression, context);
            if (ReferenceEquals(expression, node.Expression))
                return node;

            return node with { Expression = expression };
        }
    }
}
