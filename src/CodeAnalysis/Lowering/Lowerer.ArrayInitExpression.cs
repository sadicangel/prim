using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundArrayInitExpression LowerArrayInitExpression(BoundArrayInitExpression node, Context context)
    {
        var elements = LowerList(node.Elements, context, LowerExpression);
        if (elements.IsDefault)
            return node;

        return node with { Elements = new(elements) };
    }
}
