using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static IfExpressionSyntax ParseIfElseExpression(SyntaxTokenStream stream)
    {
        var ifKeyword = stream.Match(SyntaxKind.IfKeyword);
        var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression(stream);
        var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
        var then = ParseExpression(stream);
        var elseClause = ParseElseClauseExpression(stream);

        return new IfExpressionSyntax(
            ifKeyword,
            parenthesisOpenToken,
            condition,
            parenthesisCloseToken,
            then,
            elseClause);
    }
}
