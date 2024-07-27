using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundStructInitExpression LowerStructInitExpression(BoundStructInitExpression node, Context context)
    {
        var properties = LowerList(node.Properties, context, LowerPropertyInitExpression);
        if (properties.IsDefault)
            return node;

        return node with { Properties = new(properties) };

        static BoundPropertyInitExpression LowerPropertyInitExpression(BoundPropertyInitExpression node, Context context)
        {
            var expression = LowerExpression(node.Expression, context);
            if (ReferenceEquals(expression, node.Expression))
                return node;

            return node with { Expression = expression };
        }
    }
}
