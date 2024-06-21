using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindInvocationExpression(InvocationExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        if (expression.Type is not FunctionType functionType)
        {
            context.Diagnostics.ReportInvalidFunctionSymbol(syntax.Expression.Location);
            return new BoundNeverExpression(syntax);
        }

        if (functionType.Parameters.Count != syntax.Arguments.Count)
        {
            context.Diagnostics.ReportInvalidArgumentListLength(
                syntax.Location,
                functionType.Name,
                functionType.Parameters.Count,
                syntax.Arguments.Count);
            return new BoundNeverExpression(syntax);
        }

        var arguments = new BoundList<BoundExpression>.Builder(syntax.Arguments.Count);
        for (var i = 0; i < functionType.Parameters.Count; ++i)
        {
            var argument = BindArgument(syntax.Arguments[i], functionType.Parameters[i], context);
            arguments.Add(argument);
        }

        return new BoundInvocationExpression(syntax, expression, arguments.ToBoundList(), functionType.ReturnType);

        static BoundExpression BindArgument(ArgumentSyntax syntax, Parameter parameter, BinderContext context)
        {
            var expression = Coerce(BindExpression(syntax.Expression, context), parameter.Type, context);
            return expression;
        }
    }
}
