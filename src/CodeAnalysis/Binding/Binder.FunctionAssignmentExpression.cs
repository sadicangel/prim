using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundFunctionAssignmentExpression BindFunctionAssignmentExpression(
        ExpressionSyntax syntax,
        FunctionSymbol functionSymbol,
        BinderContext context)
    {
        var body = BindFunctionBody(syntax, functionSymbol, context);
        return new BoundFunctionAssignmentExpression(syntax, functionSymbol, body);
    }
}
