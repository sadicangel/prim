using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstantFolding;
partial class ConstantFolder
{
    private static object? FoldLiteralExpression(BoundLiteralExpression node) => node.Value;
}
