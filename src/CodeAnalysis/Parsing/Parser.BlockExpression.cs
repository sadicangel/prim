using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static BlockExpressionSyntax ParseBlockExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var expressions = ParseSyntaxList(
            syntaxTree,
            iterator,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParseTerminatedExpression);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);
        return new BlockExpressionSyntax(syntaxTree, braceOpenToken, expressions, braceCloseToken);
    }
}
