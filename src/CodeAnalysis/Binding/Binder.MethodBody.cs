using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindMethodBody(
        ExpressionSyntax syntax,
        MethodSymbol methodSymbol,
        BinderContext context)
    {
        using (context.PushScope())
        {
            foreach (var parameterSymbol in methodSymbol.Parameters)
            {
                // We've already reported redeclarations.
                _ = context.BoundScope.Declare(parameterSymbol);
            }

            // TODO: Check for unused parameters.
            var body = Coerce(BindExpression(syntax, context), methodSymbol.ReturnType, context);

            return body;

        }
    }
}
