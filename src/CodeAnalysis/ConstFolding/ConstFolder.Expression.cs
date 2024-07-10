using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    public static object? FoldExpression(BoundExpression node)
    {
        return node.BoundKind switch
        {
            BoundKind.LiteralExpression =>
                FoldLiteralExpression((BoundLiteralExpression)node),
            BoundKind.BlockExpression =>
                FoldBlockExpression((BoundBlockExpression)node),
            BoundKind.UnaryExpression =>
                FoldUnaryExpression((BoundUnaryExpression)node),
            BoundKind.BinaryExpression =>
                FoldBinaryExpression((BoundBinaryExpression)node),
            BoundKind.IfExpression =>
                FoldIfElseExpression((BoundIfExpression)node),
            BoundKind.VariableDeclaration or
            BoundKind.LocalReference => null,
            _ =>
                throw new NotImplementedException(node.BoundKind.ToString()),
        };
    }
}
