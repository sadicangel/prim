using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

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
        var elseClause = default(ElseClauseExpressionSyntax);
        if (iterator.TryMatch(out var elseKeyword, SyntaxKind.ElseKeyword))
        {
            var @else = ParseExpression(syntaxTree, iterator);
            elseClause = new ElseClauseExpressionSyntax(syntaxTree, elseKeyword, @else);
        }

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
