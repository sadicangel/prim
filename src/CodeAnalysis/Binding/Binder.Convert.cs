using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Convert(BoundExpression expression, PrimType type, bool isExplicit, BinderContext context)
    {
        if (type.IsAny)
            return expression;

        if (expression.Type == type)
        {
            if (isExplicit)
            {
                context.Diagnostics.ReportRedundantConversion(expression.Syntax.Location);
            }
            return expression;
        }

        var conversion = expression.Type.GetConversion(expression.Type, type)
            ?? type.GetConversion(expression.Type, type);

        if (conversion is null)
        {
            context.Diagnostics.ReportInvalidConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.Syntax);
        }

        if (!isExplicit && conversion.IsExplicit)
        {
            context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.Syntax);
        }

        var conversionSymbol = new ConversionSymbol(expression.Syntax, conversion);

        return new BoundConversionExpression(expression.Syntax, conversionSymbol, expression);
    }
}
