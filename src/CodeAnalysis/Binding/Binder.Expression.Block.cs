using System.Collections.Immutable;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundBlockExpression BindBlockExpression(BlockExpressionSyntax syntax, BindingContext context)
    {
        foreach (var declarations in syntax.Expressions.OfType<DeclarationSyntax>())
        {
            DeclarePass2(declarations, context, allowInference: true);
        }

        var types = new HashSet<TypeSymbol>();
        var expressions = ImmutableArray.CreateBuilder<BoundExpression>(syntax.Expressions.Count);
        foreach (var expressionSyntax in syntax.Expressions)
        {
            var expression = BindExpression(expressionSyntax, context);
            expressions.Add(expression);
            if (expression.CanExitScope)
                types.Add(expression.Type);
        }

        if (expressions.Count > 0)
            types.Add(expressions[^1].Type);

        var type = types switch
        {
            { Count: 0 } => context.Module.Unit,
            { Count: 1 } => types.Single(),
            _ when types.Contains(context.Module.Never) => context.Module.Never,
            _ => new UnionSymbol(syntax, [.. types], context.Module)
        };

        return new BoundBlockExpression(syntax, type, expressions.MoveToImmutable());
    }
}
