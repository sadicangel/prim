using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ArrayInitializerExpressionSyntax ParseArrayInitializerExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
        var elements = ParseSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.BracketCloseToken],
            ParseExpression);
        var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);

        return new ArrayInitializerExpressionSyntax(syntaxTree, bracketOpenToken, elements, bracketCloseToken);
    }
}
