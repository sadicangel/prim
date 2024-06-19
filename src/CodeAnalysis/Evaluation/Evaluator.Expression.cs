using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Evaluation.Values;

namespace CodeAnalysis.Evaluation;
partial class Evaluator
{
    private static PrimValue EvaluateExpression(BoundExpression node, EvaluatorContext context)
    {
        return node.BoundKind switch
        {
            BoundKind.NeverExpression =>
                EvaluateNeverExpression((BoundNeverExpression)node, context),
            BoundKind.IdentifierNameExpression =>
                EvaluateIdentifierNameExpression((BoundIdentifierNameExpression)node, context),
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
            //BoundKind.SimpleAssignmentExpression or
            //BoundKind.AddAssignmentExpression or
            //BoundKind.SubtractAssignmentExpression or
            //BoundKind.MultiplyAssignmentExpression or
            //BoundKind.DivideAssignmentExpression or
            //BoundKind.ModuloAssignmentExpression or
            //BoundKind.PowerAssignmentExpression or
            //BoundKind.AndAssignmentExpression or
            //BoundKind.ExclusiveOrAssignmentExpression or
            //BoundKind.OrAssignmentExpression or
            //BoundKind.LeftShiftAssignmentExpression or
            //BoundKind.RightShiftAssignmentExpression or
            //BoundKind.CoalesceAssignmentExpression =>
            //    EvaluateAssignmentExpression((BoundAssignmentExpression)node, context),
            BoundKind.VariableDeclaration =>
                EvaluateVariableDeclaration((BoundVariableDeclaration)node, context),
            //BoundKind.FunctionDeclaration =>
            //    EvaluateFunctionDeclaration((BoundFunctionDeclaration)node, context),
            BoundKind.StructDeclaration =>
                EvaluateStructDeclaration((BoundStructDeclaration)node, context),
            //BoundKind.LocalDeclaration =>
            //    EvaluateLocalDeclaration((BoundLocalDeclaration)node, context),
            //BoundKind.EmptyExpression =>
            //    EvaluateEmptyExpression((BoundEmptyExpression)node, context),
            //BoundKind.StatementExpression =>
            //    EvaluateStatementExpression((BoundStatementExpression)node, context),
            //BoundKind.BlockExpression =>
            //    EvaluateBlockExpression((BoundBlockExpression)node, context),
            //BoundKind.ArrayExpression =>
            //    EvaluateArrayExpression((BoundArrayExpression)node, context),
            BoundKind.StructExpression =>
                EvaluateStructExpression((BoundStructExpression)node, context),
            //BoundKind.ConversionExpression =>
            //    EvaluateConversionExpression((BoundConversionExpression)node, context),
            //BoundKind.UnaryPlusExpression or
            //BoundKind.UnaryMinusExpression or
            //BoundKind.OnesComplementExpression or
            //BoundKind.NotExpression =>
            //    EvaluateUnaryExpression((BoundUnaryExpression)node, context),
            BoundKind.AddExpression or
            BoundKind.SubtractExpression or
            BoundKind.MultiplyExpression or
            BoundKind.DivideExpression or
            BoundKind.ModuloExpression or
            BoundKind.PowerExpression or
            BoundKind.LeftShiftExpression or
            BoundKind.RightShiftExpression or
            BoundKind.LogicalOrExpression or
            BoundKind.LogicalAndExpression or
            BoundKind.BitwiseOrExpression or
            BoundKind.BitwiseAndExpression or
            BoundKind.ExclusiveOrExpression or
            BoundKind.EqualsExpression or
            BoundKind.NotEqualsExpression or
            BoundKind.LessThanExpression or
            BoundKind.LessThanOrEqualExpression or
            BoundKind.GreaterThanExpression or
            BoundKind.GreaterThanOrEqualExpression or
            BoundKind.CoalesceExpression =>
                EvaluateBinaryExpression((BoundBinaryExpression)node, context),
            _ =>
                throw new NotImplementedException(node.BoundKind.ToString()),
        };
    }
}
