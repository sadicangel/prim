using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static LambdaExpressionSyntax ParseLambdaExpression(SyntaxTokenStream stream)
    {
        var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
        var parameters = ParseSyntaxList(
            stream,
            SyntaxKind.CommaToken,
            [SyntaxKind.ParenthesisCloseToken],
            ParseSimpleName);

        var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
        var equalsGreaterThanToken = stream.Match(SyntaxKind.EqualsGreaterThanToken);
        // TODO: This should be block expression or statement expression. Not all.
        var body = ParseExpressionTerminated(stream);

        return new LambdaExpressionSyntax(
            parenthesisOpenToken,
            parameters,
            parenthesisCloseToken,
            equalsGreaterThanToken,
            body);
    }
}
