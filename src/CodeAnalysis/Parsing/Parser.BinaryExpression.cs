using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Operators;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseBinaryExpression(SyntaxTree syntaxTree, SyntaxIterator iterator, int parentPrecedence = 0)
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

begin:
        switch (iterator.Current.SyntaxKind)
        {
            case SyntaxKind.AsKeyword:
                {
                    var asKeyword = iterator.Match(SyntaxKind.AsKeyword);
                    var type = ParseType(syntaxTree, iterator);
                    left = new ConversionExpressionSyntax(syntaxTree, left, asKeyword, type);
                }
                goto begin;

            case SyntaxKind.ParenthesisOpenToken:
                {
                    var parenthesisOpenToken = iterator.Match(SyntaxKind.ParenthesisOpenToken);
                    var arguments = ParseSeparatedSyntaxList(
                        syntaxTree,
                        iterator,
                        SyntaxKind.CommaToken,
                        [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
                        ParseArgument);
                    var parenthesisCloseToken = iterator.Match(SyntaxKind.ParenthesisCloseToken);

                    left = new InvocationExpressionSyntax(syntaxTree, left, parenthesisOpenToken, arguments, parenthesisCloseToken);

                    static ArgumentSyntax ParseArgument(SyntaxTree syntaxTree, SyntaxIterator iterator)
                    {
                        var expression = ParseExpression(syntaxTree, iterator);
                        return new ArgumentSyntax(syntaxTree, expression);
                    }
                }
                goto begin;

            case SyntaxKind _ when TryParseBinaryOperator(syntaxTree, iterator, parentPrecedence, out var binaryOperator):
                {
                    var syntaxKind = SyntaxFacts.GetBinaryOperatorExpression(binaryOperator.SyntaxKind);
                    var right = ParseBinaryExpression(syntaxTree, iterator, binaryOperator.Precedence);
                    left = new BinaryExpressionSyntax(syntaxKind, syntaxTree, left, binaryOperator, right);
                }
                goto begin;
        }
        return left;

        static bool TryParseUnaryOperator(
            SyntaxTree syntaxTree,
            SyntaxIterator iterator,
            int parentPrecedence,
            [MaybeNullWhen(false)] out OperatorSyntax unaryOperator)
        {
            var (operatorKind, precedence) = SyntaxFacts.GetUnaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (operatorKind is not 0 && precedence >= parentPrecedence)
            {
                var operatorToken = iterator.Match();
                unaryOperator = new OperatorSyntax(operatorKind, syntaxTree, operatorToken, precedence);
                return true;
            }
            unaryOperator = null;
            return false;
        }

        static bool TryParseBinaryOperator(
            SyntaxTree syntaxTree,
            SyntaxIterator iterator,
            int parentPrecedence,
            [MaybeNullWhen(false)] out OperatorSyntax binaryOperator)
        {
            var (operatorKind, precedence) = SyntaxFacts.GetBinaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (operatorKind is not 0 && precedence >= parentPrecedence)
            {
                var operatorToken = iterator.Match();
                binaryOperator = new OperatorSyntax(operatorKind, syntaxTree, operatorToken, precedence);
                return true;
            }
            binaryOperator = null;
            return false;
        }
    }
}
