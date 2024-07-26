using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindContinueExpression(ContinueExpressionSyntax syntax, BinderContext context)
    {
        if (context.LoopScope?.ContinueLabel is not LabelSymbol continueLabel)
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

        return new BoundContinueExpression(syntax, continueLabel, expression);
    }
}
