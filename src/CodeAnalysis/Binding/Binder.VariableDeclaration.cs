using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not VariableSymbol variableSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(VariableDeclarationSyntax)}'");

        var expression = BindExpression(syntax.Expression, context);

        if (variableSymbol.Type.IsUnknown)
        {
            if (expression.Type.IsUnknown)
            {
                context.Diagnostics.ReportInvalidImplicitType(syntax.Location, expression.Type.Name);
                variableSymbol = variableSymbol with { Type = PredefinedTypes.Never };
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
        return new BoundVariableDeclaration(syntax, variableSymbol, expression);
    }
}
