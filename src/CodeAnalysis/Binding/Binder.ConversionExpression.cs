using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindConversionExpression(ConversionExpressionSyntax syntax, BindingContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        var type = BindType(syntax.Type, context);
        var conversion = expression.Type.GetConversion(expression.Type, type)
            ?? type.GetConversion(expression.Type, type);
        if (conversion is null)
        {
            context.Diagnostics.ReportInvalidTypeConversion(syntax.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(syntax);
        }

        var conversionSymbol = new ConversionSymbol(syntax.AsKeyword, conversion);

        return new BoundConversionExpression(syntax, conversionSymbol, expression);
    }
}
