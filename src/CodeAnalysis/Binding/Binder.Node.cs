using System.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Binding;

partial class Binder
{
    public static BoundNode Bind(this SyntaxNode syntax, BindingContext context)
    {
        return syntax switch
        {
            ExpressionSyntax expression => BindExpression(expression, context),
            _ => throw new UnreachableException($"Unexpected node type '{syntax.GetType().Name}'")
        };
    }
}
