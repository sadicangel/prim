using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundStructInitExpression LowerStructInitExpression(BoundStructInitExpression node)
    {
        var properties = LowerList(node.Properties, LowerPropertyInitExpression);
        if (properties is null)
            return node;

        return node with { Properties = new(properties) };

        static BoundPropertyInitExpression LowerPropertyInitExpression(BoundPropertyInitExpression node)
        {
            var expression = LowerExpression(node.Expression);
            if (ReferenceEquals(expression, node.Expression))
                return node;

            return node with { Expression = expression };
        }
    }
}
