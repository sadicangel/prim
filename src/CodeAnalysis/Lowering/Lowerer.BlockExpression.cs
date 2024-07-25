using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundBlockExpression LowerBlockExpression(BoundBlockExpression node)
    {
        var expressions = LowerList(node.Expressions, LowerExpression);
        if (expressions is null)
            return node;

        return node with { Expressions = new(expressions) };
    }
}
