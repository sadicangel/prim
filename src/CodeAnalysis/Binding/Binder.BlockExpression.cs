using System.Collections.Immutable;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax, Context context)
    {
        var types = new HashSet<TypeSymbol>();
        var builder = ImmutableArray.CreateBuilder<BoundExpression>(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            builder.Add(expression);
            if (expression.CanJump())
                types.Add(expression.Type);
        }

        var expressions = new BoundList<BoundExpression>(builder.ToImmutable());

        if (expressions.Count > 0)
            types.Add(expressions[^1].Type);

        var type = TypeSymbol.FromSet(types);

        return new BoundBlockExpression(syntax, type, expressions);
    }
}
