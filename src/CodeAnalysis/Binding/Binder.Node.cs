using System.Diagnostics;
using CodeAnalysis.Binding.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;
partial class Binder
{
    private static BoundExpression BindNode(SyntaxNode node, BinderContext context)
    {
        return node switch
        {
            ExpressionSyntax expression => BindExpression(expression, context),
            _ => throw new UnreachableException($"Unexpected node type '{node.GetType().Name}'")
        };
    }
}
