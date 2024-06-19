using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static GroupExpressionSyntax ParseGroupExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        // '(' expr ')'
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var expression = ParseExpression(syntaxTree, iterator);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        return new GroupExpressionSyntax(
            syntaxTree,
            parenthesisOpenToken,
            expression,
            parenthesisCloseToken);
    }
}
