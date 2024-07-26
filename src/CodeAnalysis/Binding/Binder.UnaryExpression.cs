using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax, Context context)
    {
        var operand = BindExpression(syntax.Operand, context);
        if (operand.Type.IsNever)
            return operand;

        var operators = operand.Type.GetUnaryOperators(syntax.OperatorToken.SyntaxKind, operand.Type);
        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        if (operators is not [var @operator])
        {
            // TODO: Is this case ever possible?
            context.Diagnostics.ReportAmbiguousUnaryOperator(syntax.OperatorToken, operand.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        return new BoundUnaryExpression(syntax, @operator, operand);
    }
}
