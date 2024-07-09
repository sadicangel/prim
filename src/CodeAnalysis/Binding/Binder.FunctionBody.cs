using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindFunctionBody(
        ExpressionSyntax syntax,
        FunctionSymbol functionSymbol,
        BinderContext context)
    {
        using (context.PushScope())
        {
            foreach (var parameterSymbol in functionSymbol.Parameters)
            {
                // We've already reported redeclarations.
                _ = context.BoundScope.Declare(parameterSymbol);
            }

            // TODO: Check for unused parameters.
            var body = Coerce(BindExpression(syntax, context), functionSymbol.ReturnType, context);

            return body;
        }
    }
}
