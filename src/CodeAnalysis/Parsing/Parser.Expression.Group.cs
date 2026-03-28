using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // group_expression = '(' expr ')'
    private static GroupExpressionSyntax ParseGroupExpression(SyntaxIterator iterator)
    {
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var expression = ParseExpression(iterator);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);

        return new GroupExpressionSyntax(
            parenthesisOpenToken,
            expression,
            parenthesisCloseToken);
    }
}
