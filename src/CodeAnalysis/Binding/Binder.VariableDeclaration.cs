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

        using (context.PushBoundScope())
        {
            foreach (var symbol in variableSymbol.Type.DeclaredSymbols)
                if (!context.BoundScope.Declare(symbol))
                    throw new UnreachableException($"Failed to declare symbol '{symbol}'");

            var expression = syntax.InitValue is not null
                ? BindExpression(syntax.InitValue, context)
                : null;

            if (variableSymbol.Type.IsUnknown)
            {
                if (expression is null || expression.Type.IsUnknown)
                {
                    expression ??= new BoundNeverExpression(syntax);
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
                if (expression is null)
                {
                    if (!variableSymbol.Type.IsOption)
                    {
                        context.Diagnostics.ReportUninitializedVariable(syntax.Location, variableSymbol.Name);
                        expression = new BoundNeverExpression(syntax);
                    }
                    else
                    {
                        expression = BoundLiteralExpression.Unit;
                    }
                }
                expression = Coerce(expression, variableSymbol.Type, context);
            }

            return new BoundVariableDeclaration(syntax, variableSymbol, expression);
        }
    }
}
