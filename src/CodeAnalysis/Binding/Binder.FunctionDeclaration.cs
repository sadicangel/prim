using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    private static BoundFunctionDeclaration BindFunctionDeclaration(FunctionDeclarationSyntax syntax, BinderContext context)
    {
        var symbolName = syntax.IdentifierToken.Text.ToString();
        if (context.BoundScope.Lookup(symbolName) is not FunctionSymbol functionSymbol)
            throw new UnreachableException($"Unexpected symbol for '{nameof(FunctionDeclarationSyntax)}'");

        var body = BindFunctionBody(syntax.Body, functionSymbol, context);

        var @operator = functionSymbol.Type.GetOperators(SyntaxKind.InvocationOperator).Single();
        var operatorSymbol = new OperatorSymbol(syntax, @operator);

        return new BoundFunctionDeclaration(syntax, functionSymbol, operatorSymbol, body);
    }
}
