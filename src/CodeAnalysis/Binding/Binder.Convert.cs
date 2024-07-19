using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Convert(BoundExpression expression, PrimType type, bool isExplicit, BinderContext context)
    {
        if (expression.Type.IsConvertibleTo(type, out var conversion))
        {
            if (conversion is null)
            {
                if (isExplicit)
                {
                    context.Diagnostics.ReportRedundantConversion(expression.Syntax.Location);
                }
                return expression;
            }

            if (!isExplicit && conversion.IsExplicit)
            {
                context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
                return new BoundNeverExpression(expression.Syntax);
            }

            var methodSymbol = MethodSymbol.FromConversion(conversion);

            return new BoundUnaryExpression(expression.Syntax, methodSymbol, expression);
        }

        context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
        return new BoundNeverExpression(expression.Syntax);
    }
}
