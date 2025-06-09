using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static IfExpressionSyntax ParseIfElseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var ifKeyword = iterator.Match(SyntaxKind.IfKeyword);
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression(syntaxTree, iterator);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        var then = ParseExpression(syntaxTree, iterator);
        var elseClause = ParseElseClauseExpression(syntaxTree, iterator);

        return new IfExpressionSyntax(
            syntaxTree,
            ifKeyword,
            parenthesisOpenToken,
            condition,
            parenthesisCloseToken,
            then,
            elseClause);
    }
}
