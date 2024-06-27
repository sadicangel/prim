using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindWhileExpression(WhileExpressionSyntax syntax, BinderContext context)
    {
        var condition = Coerce(BindExpression(syntax.Condition, context), PredefinedTypes.Bool, context);
        if (condition.Type.IsNever)
        {
            return condition;
        }

        var body = BindExpression(syntax.Body, context);
        if (body.Type.IsNever)
        {
            return body;
        }

        return new BoundWhileExpression(syntax, condition, body);
    }
}
