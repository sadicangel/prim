using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ArrayExpressionSyntax ParseArrayExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
        var expressions = ParseSeparatedSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.BracketCloseToken, SyntaxKind.EofToken],
            ParseExpression);
        var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);
        return new ArrayExpressionSyntax(syntaxTree, bracketOpenToken, expressions, bracketCloseToken);
    }
}
