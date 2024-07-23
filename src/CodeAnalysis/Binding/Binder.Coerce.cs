using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression Coerce(BoundExpression expression, TypeSymbol type, BinderContext context)
    {
        if (expression.Type.IsNever)
        {
            return expression;
        }

        if (expression.Type.IsCoercibleTo(type, out var conversion))
        {
            if (conversion is null)
            {
                return expression;
            }

            return new BoundUnaryExpression(expression.Syntax, conversion, expression);
        }

        if (conversion?.IsExplicitConversion is true)
        {
            context.Diagnostics.ReportInvalidImplicitConversion(expression.Syntax.Location, expression.Type.Name, type.Name);
            return new BoundNeverExpression(expression.Syntax);
        }

        context.Diagnostics.ReportInvalidExpressionType(expression.Syntax.Location, type.Name, expression.Type.Name);
        return new BoundNeverExpression(expression.Syntax);
    }
}
