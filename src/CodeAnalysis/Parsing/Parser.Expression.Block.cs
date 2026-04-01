using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static BlockExpressionSyntax ParseBlockExpression(SyntaxTokenStream stream)
    {
        var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
        var expressions = ParseSyntaxList(
            stream,
            [SyntaxKind.BraceCloseToken],
            ParseBlockExpressionStatement);
        var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);

        return new BlockExpressionSyntax(braceOpenToken, expressions, braceCloseToken);
    }
}
