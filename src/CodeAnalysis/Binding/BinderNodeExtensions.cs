using System.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Binding;

internal static class BinderNodeExtensions
{
    extension(Binder binder)
    {
        public BoundNode BindNode(SyntaxNode syntax)
        {
            return syntax switch
            {
                ExpressionSyntax expression => binder.BindExpression(expression),
                StatementSyntax statement => binder.BindStatement(statement),
                _ => throw new UnreachableException($"Unexpected node type '{syntax.GetType().Name}'")
            };
        }
    }
}
