using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindInvocationExpression(InvocationExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        if (expression.Type is not FunctionType functionType)
        {
            // TODO: Report not a function.
            return new BoundNeverExpression(syntax);
        }

        if (functionType.Parameters.Count != syntax.Arguments.Count)
        {
            // TODO: Report wrong number of arguments.
            return new BoundNeverExpression(syntax);
        }

        var arguments = new BoundList<BoundExpression>.Builder(syntax.Arguments.Count);
        foreach (var argumentSyntax in syntax.Arguments)
        {
            var argument = BindArgument(argumentSyntax, context);
            arguments.Add(argument);
        }

        return new BoundInvocationExpression(syntax, expression, arguments.ToBoundList(), functionType.ReturnType);

        static BoundExpression BindArgument(ArgumentSyntax syntax, BinderContext context)
        {
            // TODO: Do we care about anything else - like parameter name?
            var expression = BindExpression(syntax.Expression, context);
            return expression;
        }
    }
}
