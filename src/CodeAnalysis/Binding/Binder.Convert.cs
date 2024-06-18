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

        var containingType = expression.Type;
        var conversion = containingType.GetConversion(expression.Type, type);
        if (conversion is null)
        {
            containingType = type;
            containingType.GetConversion(expression.Type, type);
        }

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

        var containingSymbol = containingType is not StructType structType
            ? null
            : context.BoundScope.Lookup(structType.Name) as StructSymbol;

        var conversionSymbol = new ConversionSymbol(expression.Syntax, conversion, containingSymbol);

        return new BoundConversionExpression(expression.Syntax, conversionSymbol, expression);
    }
}
