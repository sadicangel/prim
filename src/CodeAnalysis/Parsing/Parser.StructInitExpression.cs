using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructInitExpressionSyntax ParseStructInitExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var properties = ParseSeparatedSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParsePropertyInitExpression);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);
        return new StructInitExpressionSyntax(syntaxTree, identifierToken, braceOpenToken, properties, braceCloseToken);
    }
}
