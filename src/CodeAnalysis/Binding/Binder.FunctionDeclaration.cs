using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundExpression BindFunctionDeclaration(FunctionDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not FunctionSymbol functionSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(FunctionDeclarationSyntax)}'");

        var body = BindFunctionBody(syntax.Body, functionSymbol, context);

        return new BoundFunctionDeclaration(syntax, functionSymbol, body);
    }
}
