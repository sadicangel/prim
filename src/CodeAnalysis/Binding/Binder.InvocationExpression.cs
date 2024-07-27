using System.Collections.Immutable;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindInvocationExpression(InvocationExpressionSyntax syntax, Context context)
    {
        var expression = BindExpression(syntax.Expression, context);
        var methodGroup = expression as BoundMethodGroup;

        var operators = methodGroup?.MethodSymbols.SelectMany(s => s.Type.GetOperators(SyntaxKind.ParenthesisOpenParenthesisCloseToken)).ToList()
            ?? expression.Type.GetOperators(SyntaxKind.ParenthesisOpenParenthesisCloseToken);

        if (operators is [])
        {
            context.Diagnostics.ReportUndefinedInvocationOperator(syntax.Location, expression.Type.Name);
            return new BoundNeverExpression(syntax);
        }

        operators.RemoveAll(o => o.Parameters.Count != syntax.Arguments.Count);

        if (operators is [])
        {
            context.Diagnostics.ReportInvalidArgumentListLength(syntax.Location, syntax.Arguments.Count);
            return new BoundNeverExpression(syntax);
        }

        var arguments = new BoundList<BoundExpression>(syntax.Arguments.Select(arg => BindArgument(arg, context)).ToImmutableArray());

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

        if (methodGroup is not null)
        {
            // Because we have bound to a specific method, we need to use that reference.
            expression = new BoundMethodReference(
                methodGroup.Syntax,
                methodGroup.Expression,
                methodGroup.GetMethod(@operator.LambdaType));
        }

        return new BoundInvocationExpression(syntax, expression, @operator, arguments);

        static BoundExpression BindArgument(ArgumentSyntax syntax, Context context)
        {
            var expression = BindExpression(syntax.Expression, context);
            return expression;
        }

        static List<MethodSymbol> MatchOperators(List<MethodSymbol> operators, BoundList<BoundExpression> arguments, out MethodSymbol? exactMatch)
        {
            exactMatch = null;
            var matchingOperators = new List<MethodSymbol>();
            foreach (var @operator in operators)
            {
                var allArgsCoercible = true;
                var allArgsExactType = true;
                for (var i = 0; i < @operator.Parameters.Count; ++i)
                {
                    var parameter = @operator.Parameters[i];
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
