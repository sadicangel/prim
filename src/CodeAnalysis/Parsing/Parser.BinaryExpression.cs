using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Operators;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseBinaryExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator, int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        if (TryParseUnaryOperator(syntaxTree, iterator, parentPrecedence, out var unaryOperator))
        {
            var syntaxKind = SyntaxFacts.GetUnaryOperatorExpression(unaryOperator.SyntaxKind);
            var operand = ParseBinaryExpression(syntaxTree, iterator, unaryOperator.Precedence);
            left = new UnaryExpressionSyntax(syntaxKind, syntaxTree, unaryOperator, operand);
        }
        else
        {
            left = ParsePrimaryExpression(syntaxTree, iterator);
        }

        while (true)
        {
            if (!TryParseBinaryOperator(syntaxTree, iterator, parentPrecedence, out var binaryOperator))
                break;

            var syntaxKind = SyntaxFacts.GetBinaryOperatorExpression(binaryOperator.SyntaxKind);
            var right = ParseBinaryExpression(syntaxTree, iterator, binaryOperator.Precedence);
            left = new BinaryExpressionSyntax(syntaxKind, syntaxTree, left, binaryOperator, right);

        }
        return left;

        static bool TryParseUnaryOperator(
            SyntaxTree syntaxTree,
            SyntaxTokenIterator iterator,
            int parentPrecedence,
            [MaybeNullWhen(false)] out OperatorSyntax unaryOperator)
        {
            var (operatorKind, precedence) = SyntaxFacts.GetUnaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (operatorKind is not 0 && precedence >= parentPrecedence)
            {
                var operatorToken = iterator.Next();
                unaryOperator = new OperatorSyntax(operatorKind, syntaxTree, operatorToken, precedence);
                return true;
            }
            unaryOperator = null;
            return false;
        }

        static bool TryParseBinaryOperator(
            SyntaxTree syntaxTree,
            SyntaxTokenIterator iterator,
            int parentPrecedence,
            [MaybeNullWhen(false)] out OperatorSyntax binaryOperator)
        {
            var (operatorKind, precedence) = SyntaxFacts.GetBinaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (operatorKind is not 0 && precedence >= parentPrecedence)
            {
                var operatorToken = iterator.Next();
                binaryOperator = new OperatorSyntax(operatorKind, syntaxTree, operatorToken, precedence);
                return true;
            }
            binaryOperator = null;
            return false;
        }
    }
}
