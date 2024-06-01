using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax, BindingContext context)
    {
        var expressions = new BoundList<BoundExpression>.Builder(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            expressions.Add(expression);
        }
        return new BoundBlockExpression(syntax, expressions.ToBoundList());
    }
}
