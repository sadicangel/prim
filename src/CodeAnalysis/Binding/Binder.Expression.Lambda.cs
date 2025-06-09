using System.Collections.Immutable;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundLambdaExpression BindLambdaExpression(LambdaExpressionSyntax syntax, BindingContext context)
    {
        using var scope = context.PushScope();

        var returnNever = false;
        var parameters = ImmutableArray.CreateBuilder<VariableSymbol>(syntax.Parameters.Count);
        foreach (var parameterSyntax in syntax.Parameters)
        {
            var parameterType = parameterSyntax.TypeClause?.Type is TypeSyntax typeSyntax
                ? BindType(typeSyntax, context)
                : context.Module.Unknown;

            var parameter = new VariableSymbol(parameterSyntax, parameterSyntax.Name.FullName, parameterType, context.Module, Modifiers.ReadOnly);

            if (!context.TryDeclare(parameter))
            {
                returnNever = true;
                context.Diagnostics.ReportSymbolRedeclaration(parameterSyntax.SourceSpan, parameter.Name);
            }

            parameters.Add(parameter);
        }

        var body = returnNever
            ? new BoundNeverExpression(syntax.Body, context.Module.Never)
            : BindExpression(syntax.Body, context);

        var type = new LambdaSymbol(syntax, [.. parameters.Select(x => x.Type)], body.Type, context.Module);

        return new BoundLambdaExpression(syntax, type, parameters.MoveToImmutable(), body);
    }
}
