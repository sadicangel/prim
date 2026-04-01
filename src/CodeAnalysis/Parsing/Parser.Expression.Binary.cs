using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    // binary_expression = unary-expression (binary-operator binary_expression)*
    private static ExpressionSyntax ParseBinaryExpression(SyntaxTokenStream stream, int? parentPrecedence = null)
    {
        var left = ParseUnaryExpression(stream, parentPrecedence);

begin:
        switch (stream.Current.SyntaxKind)
        {
            //case SyntaxKind.AsKeyword:
            //    {
            //        var asKeyword = stream.Match(SyntaxKind.AsKeyword);
            //        var type = ParseType(syntaxTree, stream);
            //        left = new ConversionExpressionSyntax(syntaxTree, left, asKeyword, type);
            //    }
            //    goto begin;

            //case SyntaxKind.DotToken:
            //    {
            //        var dotToken = stream.Match(SyntaxKind.DotToken);
            //        var name = ParseSimpleName(syntaxTree, stream);
            //        left = new MemberAccessExpressionSyntax(syntaxTree, left, dotToken, name);
            //    }
            //    goto begin;

            //case SyntaxKind.BraceOpenToken when left is NameSyntax name:
            //    {
            //        var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
            //        var properties = ParseSeparatedSyntaxList(
            //            syntaxTree,
            //            stream,
            //            SyntaxKind.CommaToken,
            //            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            //            ParsePropertyInitExpression);
            //        var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);
            //        left = new StructInitExpressionSyntax(syntaxTree, name, braceOpenToken, properties, braceCloseToken);
            //    }
            //    goto begin;

            //case SyntaxKind.BracketOpenToken:
            //    {
            //        var bracketOpenToken = stream.Match(SyntaxKind.BracketOpenToken);
            //        var index = ParseExpression(syntaxTree, stream);
            //        var bracketCloseToken = stream.Match(SyntaxKind.BracketCloseToken);

            //        left = new IndexExpressionSyntax(syntaxTree, left, bracketOpenToken, index, bracketCloseToken);
            //    }
            //    goto begin;

            //case SyntaxKind.ParenthesisOpenToken:
            //    {
            //        var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            //        var arguments = ParseSeparatedSyntaxList(
            //            syntaxTree,
            //            stream,
            //            SyntaxKind.CommaToken,
            //            [SyntaxKind.ParenthesisCloseToken, SyntaxKind.EofToken],
            //            ParseArgument);
            //        var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);

            //        left = new InvocationExpressionSyntax(syntaxTree, left, parenthesisOpenToken, arguments, parenthesisCloseToken);

            //        static ArgumentSyntax ParseArgument(SyntaxTree syntaxTree, SyntaxTokenStream stream)
            //        {
            //            var expression = ParseExpression(syntaxTree, stream);
            //            return new ArgumentSyntax(syntaxTree, expression);
            //        }
            //    }
            //    goto begin;

            //case SyntaxKind.EqualsToken:
            //    {
            //        var equalsToken = stream.Match(SyntaxKind.EqualsToken);
            //        var right = ParseBinaryExpression(syntaxTree, stream);
            //        left = new AssignmentExpressionSyntax(syntaxTree, left, equalsToken, right);
            //    }
            //    goto begin;

            case { } when SyntaxFacts.GetBinaryOperatorPrecedence(stream.Current.SyntaxKind) is var binaryPrecedence && binaryPrecedence >= parentPrecedence.GetValueOrDefault():
                {
                    var operatorToken = stream.Match();
                    var syntaxKind = SyntaxFacts.GetBinaryOperatorExpression(operatorToken.SyntaxKind);
                    var right = ParseBinaryExpression(stream, binaryPrecedence);
                    left = new BinaryExpressionSyntax(syntaxKind, left, operatorToken, right);
                }
                goto begin;
        }

        return left;
    }
}
