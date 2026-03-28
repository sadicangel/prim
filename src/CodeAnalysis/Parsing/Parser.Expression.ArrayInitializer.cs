using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ArrayInitializerExpressionSyntax ParseArrayInitializerExpression(SyntaxIterator iterator)
    {
        var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
        var elements = ParseSyntaxList(
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.BracketCloseToken],
            ParseExpression);
        var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);

        return new ArrayInitializerExpressionSyntax(bracketOpenToken, elements, bracketCloseToken);
    }
}
