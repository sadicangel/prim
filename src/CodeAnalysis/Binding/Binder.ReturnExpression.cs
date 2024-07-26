using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindReturnExpression(ReturnExpressionSyntax syntax, Context context)
    {
        if (context.Lambda is null)
        {
            context.Diagnostics.ReportInvalidReturn(syntax.Location);
            return new BoundNeverExpression(syntax);
        }

        var expression = syntax.Expression is null
            ? new BoundNopExpression(syntax)
            : BindExpression(syntax.Expression, context);
        if (expression.Type.IsNever)
        {
            return expression;
        }

        return new BoundReturnExpression(syntax, expression);
    }
}
