using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParsePrimaryExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var current = iterator.Current;
        return current.SyntaxKind switch
        {
            >= SyntaxKind.TrueKeyword and <= SyntaxKind.NullKeyword => ParseLiteralExpression(syntaxTree, iterator),
            >= SyntaxKind.I8LiteralToken and <= SyntaxKind.StrLiteralToken => ParseLiteralExpression(syntaxTree, iterator),
            SyntaxKind.ParenthesisOpenToken => ParseGroupExpression(syntaxTree, iterator),
            _ when iterator.Peek(1).SyntaxKind is SyntaxKind.ColonToken => ParseLocalDeclaration(syntaxTree, iterator),
            _ => ParseName(syntaxTree, iterator),
        };
    }
}
