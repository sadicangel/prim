using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundArrayExpression BindArrayExpression(ArrayExpressionSyntax syntax, BindingContext context)
    {
        PrimType type = PredefinedTypes.Unknown;
        var expressions = new BoundList<BoundExpression>.Builder(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            // TODO: Use type resolution here.
            if (!type.IsNever)
                type = expression.Type;
            expressions.Add(expression);
        }
        return new BoundArrayExpression(syntax, type, expressions.ToBoundList());
    }
}
