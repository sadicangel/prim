using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    // binary_expression = unary-expression (binary-operator binary_expression)*
    private static ExpressionSyntax ParseBinaryExpression(SyntaxTree syntaxTree, SyntaxIterator iterator, int? parentPrecedence = null)
    {
        var left = ParseUnaryExpression(syntaxTree, iterator, parentPrecedence);

begin:
        switch (iterator.Current.SyntaxKind)
        {
            case SyntaxKind when SyntaxFacts.GetBinaryOperatorPrecedence(iterator.Current.SyntaxKind) is var binaryPrecedence && binaryPrecedence >= parentPrecedence.GetValueOrDefault():
                {
                    var operatorToken = iterator.Match();
                    var syntaxKind = SyntaxFacts.GetBinaryOperatorExpression(operatorToken.SyntaxKind);
                    var right = ParseBinaryExpression(syntaxTree, iterator, binaryPrecedence);
                    left = new BinaryExpressionSyntax(syntaxKind, syntaxTree, left, operatorToken, right);
                }
                goto begin;
        }

        return left;
    }
}
