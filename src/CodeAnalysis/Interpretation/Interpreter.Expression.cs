using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Interpretation.Values;

namespace CodeAnalysis.Interpretation;
partial class Interpreter
{
    public static PrimValue EvaluateExpression(BoundExpression node, InterpreterContext context)
    {
        return node.BoundKind switch
        {
            BoundKind.NeverExpression =>
                EvaluateNeverExpression((BoundNeverExpression)node, context),
            BoundKind.LocalReference =>
                EvaluateLocalReference((BoundLocalReference)node, context),
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
                EvaluateLiteralExpression((BoundLiteralExpression)node, context),
            BoundKind.AssignmentExpression =>
                EvaluateAssignmentExpression((BoundAssignmentExpression)node, context),
            BoundKind.VariableDeclaration =>
                EvaluateVariableDeclaration((BoundVariableDeclaration)node, context),
            BoundKind.FunctionDeclaration =>
                EvaluateFunctionDeclaration((BoundFunctionDeclaration)node, context),
            BoundKind.StructDeclaration =>
                EvaluateStructDeclaration((BoundStructDeclaration)node, context),
            BoundKind.FunctionBodyExpression =>
                EvaluateFunctionBody((BoundFunctionBodyExpression)node, context),
            //BoundKind.EmptyExpression =>
            //    EvaluateEmptyExpression((BoundEmptyExpression)node, context),
            //BoundKind.StatementExpression =>
            //    EvaluateStatementExpression((BoundStatementExpression)node, context),
            BoundKind.BlockExpression =>
                EvaluateBlockExpression((BoundBlockExpression)node, context),
            BoundKind.ArrayInitExpression =>
                EvaluateArrayInitExpression((BoundArrayInitExpression)node, context),
            BoundKind.MemberReference =>
                EvaluateMemberReference((BoundMemberReference)node, context),
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
            BoundKind.IfElseExpression =>
                EvaluateIfElseExpression((BoundIfElseExpression)node, context),
            BoundKind.WhileExpression =>
                EvaluateWhileExpression((BoundWhileExpression)node, context),
            _ =>
                throw new NotImplementedException(node.BoundKind.ToString()),
        };
    }
}
