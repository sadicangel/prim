using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static AssignmentExpressionSyntax ParseAssignmentExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        // TODO: Allow non identifier expressions here.
        // TODO: Do we have reach this method?
        var left = ParseSimpleNameExpression(syntaxTree, iterator);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken);
        var right = ParseExpression(syntaxTree, iterator);

        return new AssignmentExpressionSyntax(syntaxTree, left, operatorToken, right);
    }
}
