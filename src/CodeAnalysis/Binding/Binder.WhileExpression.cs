using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindWhileExpression(WhileExpressionSyntax syntax, Context context)
    {
        var condition = Coerce(BindExpression(syntax.Condition, context), Predefined.Bool, context);
        if (condition.Type.IsNever)
        {
            return condition;
        }

        if (condition.ConstantValue is false)
        {
            context.Diagnostics.ReportUnreachableCode(syntax.Body.Location);
        }

        using (context.PushLoopScope())
        {
            var body = BindExpression(syntax.Body, context);
            if (body.Type.IsNever)
            {
                return body;
            }

            if (context.LoopScope is null)
            {
                throw new UnreachableException("Invalid loop body");
            }

            var continueLabel = context.LoopScope.ContinueLabel;
            var breakLabel = context.LoopScope.BreakLabel;

            return new BoundWhileExpression(syntax, continueLabel, condition, body, breakLabel);
        }
    }
}
