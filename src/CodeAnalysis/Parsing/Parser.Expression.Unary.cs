using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // unary_expression = unary_operator binary_expression | primary_expression
    private static ExpressionSyntax ParseUnaryExpression(SyntaxTokenStream stream, int? parentPrecedence = null)
    {
        var unaryPrecedence = SyntaxFacts.GetUnaryOperatorPrecedence(stream.Current.SyntaxKind);
        if (unaryPrecedence >= parentPrecedence.GetValueOrDefault())
        {
            var operatorToken = stream.Match();
            var syntaxKind = SyntaxFacts.GetUnaryOperatorExpression(operatorToken.SyntaxKind);
            var operand = ParseBinaryExpression(stream, unaryPrecedence);

            return new UnaryExpressionSyntax(syntaxKind, operatorToken, operand);
        }

        return ParsePrimaryExpression(stream);
    }
}
