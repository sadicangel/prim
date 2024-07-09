using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindInvocationExpression(InvocationExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        var operators = expression.Type.GetOperators(SyntaxKind.InvocationOperator);

        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedInvocationOperator(syntax.Location, expression.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        operators.RemoveAll(o => o.Type.Parameters.Count != syntax.Arguments.Count);

        if (operators is [])
        {
            context.Diagnostics.ReportInvalidArgumentListLength(syntax.Location, syntax.Arguments.Count);
            return new BoundNeverExpression(syntax);
        }

        var arguments = new BoundList<BoundExpression>(syntax.Arguments.Select(arg => BindArgument(arg, context)).ToList());

        var matchingOperators = MatchOperators(operators, arguments, out var @operator);
        if (@operator is null)
        {
            switch (matchingOperators)
            {
                case { Count: 0 }:
                    // TODO: Report first non matching argument instead.
                    context.Diagnostics.ReportInvalidArgumentListLength(syntax.Location, syntax.Arguments.Count);
                    return new BoundNeverExpression(syntax);

                case { Count: > 1 }:
                    context.Diagnostics.ReportAmbiguousInvocationOperator(syntax.Location, [.. arguments.Select(a => a.Type.Name)]);
                    return new BoundNeverExpression(syntax);

                default:
                    @operator = matchingOperators.Single();
                    break;
            }
        }

        var functionSymbol = FunctionSymbol.FromOperator(@operator);

        return new BoundInvocationExpression(syntax, expression, functionSymbol, arguments);

        static BoundExpression BindArgument(ArgumentSyntax syntax, BinderContext context)
        {
            var expression = BindExpression(syntax.Expression, context);
            return expression;
        }

        static List<Operator> MatchOperators(List<Operator> operators, BoundList<BoundExpression> arguments, out Operator? exactMatch)
        {
            exactMatch = null;
            var matchingOperators = new List<Operator>();
            foreach (var @operator in operators)
            {
                var allArgsCoercible = true;
                var allArgsExactType = true;
                for (var i = 0; i < @operator.Type.Parameters.Count; ++i)
                {
                    var parameter = @operator.Type.Parameters[i];
                    var argument = arguments[i];
                    allArgsExactType &= parameter.Type == argument.Type;
                    if (!argument.Type.IsCoercibleTo(parameter.Type))
                    {
                        allArgsCoercible = false;
                        break;
                    }
                }
                if (allArgsCoercible)
                {
                    matchingOperators.Add(@operator);
                    if (allArgsExactType)
                    {
                        exactMatch = @operator;
                        break;
                    }
                }
            }
            return matchingOperators;
        }
    }
}
