using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindConversionExpression(ConversionExpressionSyntax syntax, BindingContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        var type = BindType(syntax.Type, context);

        return Convert(expression, type, isExplicit: true, context);
    }
}
