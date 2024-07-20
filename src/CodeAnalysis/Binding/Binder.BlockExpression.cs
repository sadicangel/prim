using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax, BinderContext context)
    {
        TypeSymbol type = PredefinedSymbols.Unit;
        var expressions = new BoundList<BoundExpression>.Builder(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            expressions.Add(expression);
            type = expression.Type;
        }
        return new BoundBlockExpression(syntax, type, expressions.ToBoundList());
    }
}
