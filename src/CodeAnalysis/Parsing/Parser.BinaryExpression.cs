using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseBinaryExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator, int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var (unaryExpression, unaryPrecedence) = SyntaxFacts.GetUnaryOperatorPrecedence(iterator.Current.SyntaxKind);
        if (unaryExpression != 0 && unaryPrecedence >= parentPrecedence)
        {
            var operationToken = iterator.Next();
            var operand = ParseBinaryExpression(syntaxTree, iterator, unaryPrecedence);
            left = new UnaryExpressionSyntax(unaryExpression, syntaxTree, operationToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression(syntaxTree, iterator);
        }

        while (true)
        {
            var (binaryExpression, binaryPrecedence) = SyntaxFacts.GetBinaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (binaryExpression is 0 || binaryPrecedence <= parentPrecedence)
                break;

            var @operator = iterator.Next();
            var right = ParseBinaryExpression(syntaxTree, iterator, binaryPrecedence);
            left = new BinaryExpressionSyntax(binaryExpression, syntaxTree, left, @operator, right);

        }
        return left;
    }
}
