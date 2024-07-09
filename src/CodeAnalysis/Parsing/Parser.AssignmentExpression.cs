using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static AssignmentExpressionSyntax ParseAssignmentExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        // TODO: Allow non identifier expressions here.
        var left = ParseIdentifierNameExpression(syntaxTree, iterator);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken);
        var right = ParseExpression(syntaxTree, iterator);

        return new AssignmentExpressionSyntax(syntaxTree, left, operatorToken, right);
    }
}
