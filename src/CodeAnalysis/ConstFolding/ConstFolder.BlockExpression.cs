using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    private static object? FoldBlockExpression(BoundBlockExpression node)
    {
        return node.Expressions switch
        {
        [] => Unit.Value,
        [var single] => single.ConstValue,
            _ => null
        };
    }
}
