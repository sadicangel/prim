using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundFunctionDeclaration BindFunctionDeclaration(FunctionDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not FunctionSymbol symbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(FunctionDeclarationSyntax)}'");

        using (context.PushScope())
        {
            var body = BindExpression(syntax.Body, context);
            return new BoundFunctionDeclaration(syntax, symbol, body);
        }
    }
}
