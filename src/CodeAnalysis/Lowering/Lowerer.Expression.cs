using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundExpression LowerExpression(BoundExpression node)
    {
        return node.BoundKind switch
        {
            BoundKind.NeverExpression => LowerNeverExpression((BoundNeverExpression)node),
            BoundKind.LiteralExpression => LowerLiteralExpression((BoundLiteralExpression)node),
            BoundKind.AssignmentExpression => LowerAssignmentExpression((BoundAssignmentExpression)node),
            BoundKind.VariableDeclaration => LowerVariableDeclaration((BoundVariableDeclaration)node),
            BoundKind.StructDeclaration => LowerStructDeclaration((BoundStructDeclaration)node),
            BoundKind.LocalReference => LowerLocalReference((BoundLocalReference)node),
            BoundKind.PropertyReference => LowerPropertyReference((BoundPropertyReference)node),
            BoundKind.MethodReference => LowerMethodReference((BoundMethodReference)node),
            BoundKind.MethodGroup => LowerMethodGroup((BoundMethodGroup)node),
            BoundKind.IndexReference => LowerIndexReference((BoundIndexReference)node),
            BoundKind.BlockExpression => LowerBlockExpression((BoundBlockExpression)node),
            BoundKind.ArrayInitExpression => LowerArrayInitExpression((BoundArrayInitExpression)node),
            BoundKind.StructInitExpression => LowerStructInitExpression((BoundStructInitExpression)node),
            BoundKind.InvocationExpression => LowerInvocationExpression((BoundInvocationExpression)node),
            BoundKind.UnaryExpression => LowerUnaryExpression((BoundUnaryExpression)node),
            BoundKind.BinaryExpression => LowerBinaryExpression((BoundBinaryExpression)node),
            BoundKind.IfExpression => LowerIfExpression((BoundIfExpression)node),
            BoundKind.WhileExpression => LowerWhileExpression((BoundWhileExpression)node),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'"),
        };
    }
}
