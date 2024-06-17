using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindGroupExpression(GroupExpressionSyntax syntax, BinderContext context)
    {
        var expression = BindExpression(syntax.Expression, context);
        // We can just bind the actual expression because GroupExpressionSyntax
        // is just an ExpressionSyntax surrounded by parenthesis tokens.
        return expression;
    }
}
