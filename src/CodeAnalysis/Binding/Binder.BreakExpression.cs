using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions.ControlFlow;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindBreakExpression(BreakExpressionSyntax syntax, Context context)
    {
        if (context.LoopScope?.BreakLabel is not LabelSymbol breakLabel)
        {
            context.Diagnostics.ReportInvalidBreakOrContinue(syntax.Location);
            return new BoundNeverExpression(syntax, context.BoundScope.Never);
        }

        var expression = syntax.Expression is null
            ? new BoundNopExpression(syntax, context.BoundScope.Unknown)
            : BindExpression(syntax.Expression, context);

        if (expression.Type.IsNever)
        {
            return expression;
        }

        return new BoundBreakExpression(syntax, breakLabel, expression);
    }
}
