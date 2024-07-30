using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindConversionExpression(ConversionExpressionSyntax syntax, Context context)
    {
        var expression = BindExpression(syntax.Expression, context);
        var type = BindType(syntax.Type, context);

        if (expression.Type == type)
        {
            context.Diagnostics.ReportRedundantConversion(expression.Syntax.Location);
            return expression;
        }

        return Convert(expression, type, isExplicit: true, context);
    }
}
