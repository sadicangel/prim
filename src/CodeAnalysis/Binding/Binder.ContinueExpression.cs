using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindContinueExpression(ContinueExpressionSyntax syntax, Context context)
    {
        if (context.LoopScope?.ContinueLabel is not LabelSymbol continueLabel)
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

        return new BoundContinueExpression(syntax, continueLabel, expression);
    }
}
