using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax, Context context)
    {
        var left = BindExpression(syntax.Left, context);
        var right = BindExpression(syntax.Right, context);

        var containingType = left.Type;
        var operators = containingType.GetBinaryOperators(syntax.OperatorToken.SyntaxKind, left.Type, right.Type);
        if (operators is [])
        {
            containingType = right.Type;
            operators = containingType.GetBinaryOperators(syntax.OperatorToken.SyntaxKind, left.Type, right.Type);
            if (operators is [])
            {
                context.Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken, left.Type.Name, right.Type.Name);
                return new BoundNeverExpression(syntax, context.BoundScope.Never);
            }
        }

        if (operators is not [var @operator])
        {
            context.Diagnostics.ReportAmbiguousBinaryOperator(syntax.OperatorToken, left.Type.Name, right.Type.Name);
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        return new BoundBinaryExpression(syntax, left, @operator, right);
    }
}
