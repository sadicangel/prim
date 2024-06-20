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

        using (context.PushScope())
        {
            foreach (var parameter in functionSymbol.Type.Parameters)
            {
                context.BoundScope.Declare(
                    new VariableSymbol(
                        functionSymbol.Syntax,
                        parameter.Name,
                        parameter.Type,
                        IsReadOnly: false));
            }
            var body = Coerce(BindExpression(syntax.Body, context), functionSymbol.Type.ReturnType, context);

            if (body.Type.IsNever)
            {
                return body;
            }

            return new BoundFunctionDeclaration(syntax, functionSymbol, body);
        }
    }
}
