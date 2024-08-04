using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;
using CodeAnalysis.Syntax.Expressions.ControlFlow;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseBinaryExpression(SyntaxTree syntaxTree, SyntaxIterator iterator, int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        if (TryParseUnaryOperator(syntaxTree, iterator, parentPrecedence, out var unaryOperator, out var unaryPrecedence))
        {
            var syntaxKind = SyntaxFacts.GetUnaryOperatorExpression(unaryOperator.SyntaxKind);
            var operand = ParseBinaryExpression(syntaxTree, iterator, unaryPrecedence);
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

            case SyntaxKind.DotToken:
                {
                    var dotToken = iterator.Match(SyntaxKind.DotToken);
                    var name = ParseSimpleName(syntaxTree, iterator);
                    left = new MemberAccessExpressionSyntax(syntaxTree, left, dotToken, name);
                }
                goto begin;

            case SyntaxKind.BracketOpenToken:
                {
                    var bracketOpenToken = iterator.Match(SyntaxKind.BracketOpenToken);
                    var index = ParseExpression(syntaxTree, iterator);
                    var bracketCloseToken = iterator.Match(SyntaxKind.BracketCloseToken);

                    left = new IndexExpressionSyntax(syntaxTree, left, bracketOpenToken, index, bracketCloseToken);
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

            case SyntaxKind.EqualsToken:
                {
                    var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
                    var right = ParseBinaryExpression(syntaxTree, iterator);
                    left = new AssignmentExpressionSyntax(syntaxTree, left, equalsToken, right);
                }
                goto begin;

            // TODO: Support match expressions.
            case SyntaxKind _ when TryParseBinaryOperator(syntaxTree, iterator, parentPrecedence, out var binaryOperator, out var binaryPrecedence):
                {
                    var syntaxKind = SyntaxFacts.GetBinaryOperatorExpression(binaryOperator.SyntaxKind);
                    var right = ParseBinaryExpression(syntaxTree, iterator, binaryPrecedence);
                    left = new BinaryExpressionSyntax(syntaxKind, syntaxTree, left, binaryOperator, right);
                }
                goto begin;
        }
        return left;

        static bool TryParseUnaryOperator(
            SyntaxTree syntaxTree,
            SyntaxIterator iterator,
            int parentPrecedence,
            [MaybeNullWhen(false)] out SyntaxToken operatorToken,
            out int precedence)
        {
            precedence = SyntaxFacts.GetUnaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (precedence is not 0 && precedence >= parentPrecedence)
            {
                operatorToken = iterator.Match();
                return true;
            }
            operatorToken = null;
            return false;
        }

        static bool TryParseBinaryOperator(
            SyntaxTree syntaxTree,
            SyntaxIterator iterator,
            int parentPrecedence,
            [MaybeNullWhen(false)] out SyntaxToken operatorToken,
            out int precedence)
        {
            precedence = SyntaxFacts.GetBinaryOperatorPrecedence(iterator.Current.SyntaxKind);
            if (precedence is not 0 && precedence >= parentPrecedence)
            {
                operatorToken = iterator.Match();
                return true;
            }
            operatorToken = null;
            return false;
        }
    }
}
