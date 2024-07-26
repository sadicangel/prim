using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax, Context context)
    {
        var types = new HashSet<TypeSymbol>();
        var expressions = new BoundList<BoundExpression>.Builder(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            expressions.Add(expression);
            if (expression.CanJump())
                types.Add(expression.Type);
        }

        if (expressions.Count > 0)
            types.Add(expressions.Last().Type);

        var type = TypeSymbol.FromSet(types);

        return new BoundBlockExpression(syntax, type, expressions.ToBoundList());
    }
}
