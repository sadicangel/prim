using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;
internal static class Conversion
{
    public static BoundExpression Convert(this BoundExpression expression, TypeSymbol type, BindingContext context)
    {
        if (expression.Type.MapsToNever || expression.Type == type)
        {
            return expression;
        }

        //if (expression.Type.IsCoercibleTo(type, out var conversion))
        //{
        //    if (conversion is null)
        //    {
        //        return new BoundStackInstantiation(expression.Syntax, expression, type);
        //    }

        //    return new BoundConversionExpression(expression.Syntax, conversion, expression);
        //}

        //if (conversion?.IsExplicit is true)
        //{
        //    context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.SourceSpan, expression.Type.Name, type.Name);
        //    return new BoundNeverExpression(expression.Syntax, context.Module.Never);
        //}

        context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.SourceSpan, type.Name, expression.Type.Name);
        return new BoundNeverExpression(expression.Syntax, context.Module.Never);
    }
}
