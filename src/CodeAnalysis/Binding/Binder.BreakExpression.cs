using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBreakExpression(BreakExpressionSyntax syntax, BinderContext context)
    {
        if (context.LoopScope?.BreakLabel is not LabelSymbol breakLabel)
        {
            context.Diagnostics.ReportInvalidBreakOrContinue(syntax.Location);
            return new BoundNeverExpression(syntax);
        }

        var expression = syntax.Expression is null
            ? new BoundNopExpression(syntax)
            : BindExpression(syntax.Expression, context);

        if (expression.Type.IsNever)
        {
            return expression;
        }

        return new BoundBreakExpression(syntax, breakLabel, expression);
    }
}
