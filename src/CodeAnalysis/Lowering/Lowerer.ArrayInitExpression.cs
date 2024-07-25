using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundArrayInitExpression LowerArrayInitExpression(BoundArrayInitExpression node)
    {
        var elements = LowerList(node.Elements, LowerExpression);
        if (elements is null)
            return node;

        return node with { Elements = new(elements) };
    }
}
