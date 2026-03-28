using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal static class BinderConvertExtensions
{
    extension(Binder binder)
    {
        public BoundExpression Convert(BoundExpression expression, TypeSymbol targetType)
        {
            if (expression.Type.MapsToNever || expression.Type == targetType)
            {
                return expression;
            }

            //if (expression.Type.IsCoercibleTo(targetType, out var conversion))
            //{
            //    if (conversion is null)
            //    {
            //        return new BoundStackInstantiation(expression.Syntax, expression, targetType);
            //    }

            //    return new BoundConversionExpression(expression.Syntax, conversion, expression);
            //}

            //if (conversion?.IsExplicit is true)
            //{
            //    binder.ReportInvalidImplicitConversion(expression.Syntax.SourceSpan, expression.Type.Name, targetType.Name);
            //    return new BoundNeverExpression(expression.Syntax, context.Module.Never);
            //}

            binder.ReportInvalidExpressionType(expression.Syntax.SourceSpan, targetType.Name, expression.Type.Name);
            return new BoundNeverExpression(expression.Syntax, binder.Module.Never);
        }
    }
}
