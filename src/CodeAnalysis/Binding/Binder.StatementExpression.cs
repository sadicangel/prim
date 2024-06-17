using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindStatementExpression(StatementExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        // We can just bind the actual expression because StatementExpressionSyntax
        // is just an ExpressionSyntax followed by a SemicolonToken.
        return expression;
    }
}
