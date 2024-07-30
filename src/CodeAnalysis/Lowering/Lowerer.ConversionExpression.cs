using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundConversionExpression LowerConversionExpression(BoundConversionExpression node, Context context)
    {
        var expression = LowerExpression(node.Expression, context);
        if (ReferenceEquals(expression, node.Expression))
            return node;

        return node with { Expression = expression };
    }
}
