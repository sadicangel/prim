using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstantFolding;
partial class ConstantFolder
{
    private static object? FoldBlockExpression(BoundBlockExpression node)
    {
        return node.Expressions switch
        {
        [] => Unit.Value,
        [var single] => single.ConstantValue,
            _ => null
        };
    }
}
