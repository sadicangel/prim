using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.ConstFolding;
partial class ConstFolder
{
    public static object? FoldExpression(BoundExpression node)
    {
        return node.BoundKind switch
        {
            BoundKind.I32LiteralExpression or
            BoundKind.U32LiteralExpression or
            BoundKind.I64LiteralExpression or
            BoundKind.U64LiteralExpression or
            BoundKind.F32LiteralExpression or
            BoundKind.F64LiteralExpression or
            BoundKind.StrLiteralExpression or
            BoundKind.TrueLiteralExpression or
            BoundKind.FalseLiteralExpression or
            BoundKind.NullLiteralExpression =>
                FoldLiteralExpression((BoundLiteralExpression)node),
            BoundKind.BlockExpression =>
                FoldBlockExpression((BoundBlockExpression)node),
            BoundKind.UnaryExpression =>
                FoldUnaryExpression((BoundUnaryExpression)node),
            BoundKind.BinaryExpression =>
                FoldBinaryExpression((BoundBinaryExpression)node),
            BoundKind.IfElseExpression =>
                FoldIfElseExpression((BoundIfElseExpression)node),
            _ =>
                throw new NotImplementedException(node.BoundKind.ToString()),
        };
    }
}
