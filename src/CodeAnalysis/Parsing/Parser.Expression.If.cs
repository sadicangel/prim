using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static IfExpressionSyntax ParseIfElseExpression(SyntaxIterator iterator)
    {
        var ifKeyword = iterator.Match(SyntaxKind.IfKeyword);
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression(iterator);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        var then = ParseExpression(iterator);
        var elseClause = ParseElseClauseExpression(iterator);

        return new IfExpressionSyntax(
            ifKeyword,
            parenthesisOpenToken,
            condition,
            parenthesisCloseToken,
            then,
            elseClause);
    }
}
