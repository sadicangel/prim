using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    // unary_expression = unary_operator binary_expression | primary_expression
    private static ExpressionSyntax ParseUnaryExpression(SyntaxTree syntaxTree, SyntaxIterator iterator, int? parentPrecedence = null)
    {
        var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(iterator.Current.SyntaxKind);
        if (unaryPrecedence >= parentPrecedence.GetValueOrDefault())
        {
            var operatorToken = iterator.Match();
            var syntaxKind = SyntaxFacts.GetUnaryOperatorExpression(operatorToken.SyntaxKind);
            var operand = ParseBinaryExpression(syntaxTree, iterator, unaryPrecedence);

            return new UnaryExpressionSyntax(syntaxKind, syntaxTree, operatorToken, operand);
        }

        return ParsePrimaryExpression(syntaxTree, iterator);
    }
}
