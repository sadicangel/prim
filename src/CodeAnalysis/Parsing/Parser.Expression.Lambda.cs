using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static LambdaExpressionSyntax ParseLambdaExpression(SyntaxIterator iterator)
    {
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var parameters = ParseSyntaxList(
            iterator,
            SyntaxKind.CommaToken,
            [SyntaxKind.ParenthesisCloseToken],
            ParseSimpleName);

        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        var equalsGreaterThanToken = iterator.Match(SyntaxKind.EqualsGreaterThanToken);
        // TODO: This should be block expression or statement expression. Not all.
        var body = ParseExpressionTerminated(iterator);

        return new LambdaExpressionSyntax(
            parenthesisOpenToken,
            parameters,
            parenthesisCloseToken,
            equalsGreaterThanToken,
            body);
    }
}
