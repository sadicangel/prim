using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructExpressionSyntax ParseStructExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var members = ParseSeparatedSyntaxList(
            syntaxTree,
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParseMemberExpression);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);
        return new StructExpressionSyntax(syntaxTree, braceOpenToken, members, braceCloseToken);
    }
}
