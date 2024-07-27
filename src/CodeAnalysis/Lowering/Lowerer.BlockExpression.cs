using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundBlockExpression LowerBlockExpression(BoundBlockExpression node, Context context)
    {
        var expressions = LowerList(node.Expressions, context, LowerExpression);
        if (expressions.IsDefault)
            return node;

        return node with { Expressions = new(expressions) };
    }
}
