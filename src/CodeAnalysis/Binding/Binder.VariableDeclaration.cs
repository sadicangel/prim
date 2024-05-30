using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundVariableDeclaration BindVariableDeclaration(VariableDeclarationSyntax syntax, BindingContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not VariableSymbol symbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(VariableDeclarationSyntax)}'");
        var expression = BindExpression(syntax.Expression, context);
        return new BoundVariableDeclaration(syntax, symbol, expression);
    }
}
