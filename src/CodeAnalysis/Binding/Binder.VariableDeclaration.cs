using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.Name.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not VariableSymbol variableSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(VariableDeclarationSyntax)}'");

        BoundExpression expression;
        if (variableSymbol.Type is LambdaTypeSymbol lambdaType)
        {
            using (context.PushScope())
            {
                foreach (var parameter in lambdaType.Parameters)
                    if (!context.BoundScope.Declare(parameter))
                        throw new UnreachableException($"Failed to declare parameter '{parameter}'");

                expression = BindExpression(syntax.Expression, context);
                expression = Coerce(expression, lambdaType.ReturnType, context);
            }
        }
        else
        {
            expression = BindExpression(syntax.Expression, context);

            if (variableSymbol.Type.IsUnknown)
            {
                if (expression.Type.IsUnknown)
                {
                    context.Diagnostics.ReportInvalidImplicitType(syntax.Location, expression.Type.Name);
                    variableSymbol = variableSymbol with { Type = PredefinedSymbols.Never };
                }
                else
                {
                    variableSymbol = variableSymbol with { Type = expression.Type };
                }
                context.BoundScope.Replace(variableSymbol);
            }
            else
            {
                expression = Coerce(expression, variableSymbol.Type, context);
            }
        }

        return new BoundVariableDeclaration(syntax, variableSymbol, expression);
    }
}
