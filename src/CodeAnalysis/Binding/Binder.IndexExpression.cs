using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindIndexExpression(IndexExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        // TODO: Consider using an operator instead?
        if (expression.Type is not ArrayType arrayType)
        {
            context.Diagnostics.ReportInvalidArray(syntax.Expression.Location);
            return new BoundNeverExpression(syntax);
        }

        var index = Coerce(BindExpression(syntax.Index, context), PredefinedTypes.I32, context);
        if (index.Type.IsNever)
        {
            return index;
        }

        // TODO: If index is const, check bounds.

        return new BoundIndexExpression(syntax, expression, index, arrayType.ElementType);
    }
}
