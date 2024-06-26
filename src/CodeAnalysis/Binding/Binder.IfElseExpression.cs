using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindIfElseExpression(IfElseExpressionSyntax syntax, BinderContext context)
    {
        var condition = Coerce(BindExpression(syntax.Condition, context), PredefinedTypes.Bool, context);
        if (condition.Type.IsNever)
        {
            return condition;
        }

        var then = BindExpression(syntax.Then, context);
        if (then.Type.IsNever)
        {
            return then;
        }

        var @else = BindExpression(syntax.Else, context);
        if (@else.Type.IsNever)
        {
            return @else;
        }

        var type = then.Type == @else.Type ? then.Type : new UnionType([then.Type, @else.Type]);

        return new BoundIfElseExpression(syntax, condition, then, @else, type);
    }
}
