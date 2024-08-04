using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.ControlFlow;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static WhileExpressionSyntax ParseWhileExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var whileKeyword = iterator.Match(SyntaxKind.WhileKeyword);
        var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
        var condition = ParseExpression(syntaxTree, iterator);
        var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);
        var body = ParseTerminatedExpression(syntaxTree, iterator);
        return new WhileExpressionSyntax(syntaxTree, whileKeyword, parenthesisOpenToken, condition, parenthesisCloseToken, body);
    }
}
