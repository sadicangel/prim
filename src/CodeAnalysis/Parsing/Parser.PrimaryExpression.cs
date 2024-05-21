using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var curr = iterator.Current.SyntaxKind;
        var next = iterator.Peek(1).SyntaxKind;
        return curr switch
        {
            SyntaxKind.ParenthesisOpenToken => ParseGroupExpression(syntaxTree, iterator),
            _ when SyntaxFacts.IsLiteral(curr) => ParseLiteralExpression(syntaxTree, iterator),
            _ => next switch
            {
                SyntaxKind.ColonToken => ParseLocalDeclaration(syntaxTree, iterator),
                _ when SyntaxFacts.IsAssignmentOperator(next) => ParseAssignmentExpression(syntaxTree, iterator),
                _ => ParseIdentifierNameExpression(syntaxTree, iterator),
            }
        };
    }
}
