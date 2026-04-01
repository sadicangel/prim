using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // group_expression = '(' expr ')'
    private static GroupExpressionSyntax ParseGroupExpression(SyntaxTokenStream stream)
    {
        var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
        var expression = ParseExpression(stream);
        var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);

        return new GroupExpressionSyntax(
            parenthesisOpenToken,
            expression,
            parenthesisCloseToken);
    }
}
