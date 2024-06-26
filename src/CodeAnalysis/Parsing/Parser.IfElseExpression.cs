using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static IfElseExpressionSyntax ParseIfElseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var ifKeyword = iterator.Match(SyntaxKind.IfKeyword);
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression(syntaxTree, iterator);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        var then = ParseExpression(syntaxTree, iterator);
        var elseKeyword = iterator.Match(SyntaxKind.ElseKeyword);
        var @else = ParseExpression(syntaxTree, iterator);

        return new IfElseExpressionSyntax(
            syntaxTree,
            ifKeyword,
            parenthesisOpenToken,
            condition,
            parenthesisCloseToken,
            then,
            elseKeyword,
            @else);
    }
}
