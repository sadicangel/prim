using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Coerce(BoundExpression expression, PrimType type, BinderContext context)
    {
        if (type.IsAny || expression.Type.IsNever)
            return expression;

        if (expression.Type == type)
        {
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
            context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
            return new BoundNeverExpression(expression.Syntax);
        }

        if (conversion.IsExplicit)
        {
            //context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
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
