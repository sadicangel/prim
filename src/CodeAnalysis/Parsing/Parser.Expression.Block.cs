using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static BlockExpressionSyntax ParseBlockExpression(SyntaxIterator iterator)
    {
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var expressions = ParseSyntaxList(
            iterator,
            [SyntaxKind.BraceCloseToken],
            ParseBlockExpressionStatement);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new BlockExpressionSyntax(braceOpenToken, expressions, braceCloseToken);
    }
}
