using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateExpression(BoundExpression node, Context context)
    {
        return node.BoundKind switch
        {
            BoundKind.NopExpression =>
                EvaluateNopExpression((BoundNopExpression)node, context),
            BoundKind.NeverExpression =>
                EvaluateNeverExpression((BoundNeverExpression)node, context),
            BoundKind.LocalReference =>
                EvaluateLocalReference((BoundLocalReference)node, context),
            BoundKind.LiteralExpression =>
                EvaluateLiteralExpression((BoundLiteralExpression)node, context),
            BoundKind.AssignmentExpression =>
                EvaluateAssignmentExpression((BoundAssignmentExpression)node, context),
            BoundKind.LabelDeclaration =>
                EvaluateLabelDeclaration((BoundLabelDeclaration)node, context),
            BoundKind.StructDeclaration =>
                EvaluateStructDeclaration((BoundStructDeclaration)node, context),
            BoundKind.VariableDeclaration =>
                EvaluateVariableDeclaration((BoundVariableDeclaration)node, context),
            //BoundKind.EmptyExpression =>
            //    EvaluateEmptyExpression((BoundEmptyExpression)node, context),
            //BoundKind.StatementExpression =>
            //    EvaluateStatementExpression((BoundStatementExpression)node, context),
            BoundKind.BlockExpression =>
                EvaluateBlockExpression((BoundBlockExpression)node, context),
            BoundKind.ArrayInitExpression =>
                EvaluateArrayInitExpression((BoundArrayInitExpression)node, context),
            BoundKind.PropertyReference =>
                EvaluatePropertyReference((BoundPropertyReference)node, context),
            BoundKind.MethodReference =>
                EvaluateMethodReference((BoundMethodReference)node, context),
            BoundKind.MethodGroup =>
                EvaluateMethodGroup((BoundMethodGroup)node, context),
            BoundKind.IndexReference =>
                EvaluateIndexReference((BoundIndexReference)node, context),
            BoundKind.InvocationExpression =>
                EvaluateInvocationExpression((BoundInvocationExpression)node, context),
            BoundKind.StructInitExpression =>
                EvaluateStructInitExpression((BoundStructInitExpression)node, context),
            BoundKind.ConversionExpression =>
                EvaluateConversionExpression((BoundConversionExpression)node, context),
            BoundKind.UnaryExpression =>
                EvaluateUnaryExpression((BoundUnaryExpression)node, context),
            BoundKind.BinaryExpression =>
                EvaluateBinaryExpression((BoundBinaryExpression)node, context),
            //BoundKind.ReturnExpression =>
            //    EvaluateReturnExpression((BoundReturnExpression)node, context),
            BoundKind.GotoExpression =>
                EvaluateGotoExpression((BoundGotoExpression)node, context),
            BoundKind.ConditionalGotoExpression =>
                EvaluateConditionalGotoExpression((BoundConditionalGotoExpression)node, context),
            _ =>
                throw new NotImplementedException(node.BoundKind.ToString()),
        };
    }
}
