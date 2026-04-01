using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ArrayInitializerExpressionSyntax ParseArrayInitializerExpression(SyntaxTokenStream stream)
    {
        var bracketOpenToken = stream.Match(SyntaxKind.BracketOpenToken);
        var elements = ParseSyntaxList(
            stream,
            SyntaxKind.CommaToken,
            [SyntaxKind.BracketCloseToken],
            ParseExpression);
        var bracketCloseToken = stream.Match(SyntaxKind.BracketCloseToken);

        return new ArrayInitializerExpressionSyntax(bracketOpenToken, elements, bracketCloseToken);
    }
}
