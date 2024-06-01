using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructExpressionSyntax ParseStructExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var properties = ParseSeparatedSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParsePropertyExpression);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);
        return new StructExpressionSyntax(syntaxTree, identifierToken, braceOpenToken, properties, braceCloseToken);
    }
}
