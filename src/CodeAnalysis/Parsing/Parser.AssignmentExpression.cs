using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Operators;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static AssignmentExpressionSyntax ParseAssignmentExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        // TODO: Allow non identifier expressions here.
        var left = ParseIdentifierNameExpression(syntaxTree, iterator);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken);
        var @operator = new OperatorSyntax(SyntaxKind.AssignmentOperator, syntaxTree, operatorToken, Precedence: 0);
        var right = ParseExpression(syntaxTree, iterator);

        return new AssignmentExpressionSyntax(syntaxTree, left, @operator, right);
    }
}
