using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    private static object? FoldLiteralExpression(BoundLiteralExpression node) => node.Value;
}
