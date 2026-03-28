using System.Diagnostics;
using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

internal static class BinderNodeExtensions
{
    extension(Binder binder)
    {
        public BoundExpression BindNode(SyntaxNode syntax)
        {
            return syntax switch
            {
                ExpressionSyntax expression => binder.BindExpression(expression),
                _ => throw new UnreachableException($"Unexpected node type '{syntax.GetType().Name}'")
            };
        }
    }
}
