using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static GroupExpressionSyntax ParseGroupExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        // '(' expr ')'
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        return new GroupExpressionSyntax(
            syntaxTree,
            parenthesisOpenToken,
            ParseExpression(syntaxTree, iterator, isTerminated: false),
            parenthesisCloseToken);
    }
}
