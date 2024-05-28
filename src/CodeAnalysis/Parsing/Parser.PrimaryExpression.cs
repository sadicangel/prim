using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var current = iterator.Current;
        return current.SyntaxKind switch
        {
            >= SyntaxKind.TrueKeyword and <= SyntaxKind.NullKeyword => ParseLiteralExpression(syntaxTree, iterator),
            >= SyntaxKind.I32LiteralToken and <= SyntaxKind.StrLiteralToken => ParseLiteralExpression(syntaxTree, iterator),
            SyntaxKind.ParenthesisOpenToken => ParseGroupExpression(syntaxTree, iterator),
            _ => iterator.Peek(1).SyntaxKind switch
            {
                SyntaxKind.ColonToken => ParseLocalDeclaration(syntaxTree, iterator),
                SyntaxKind @operator when SyntaxFacts.IsAssignmentOperator(@operator) => ParseAssignmentExpression(syntaxTree, iterator),
                _ => ParseIdentifierNameExpression(syntaxTree, iterator),
            }
        };
    }
}
