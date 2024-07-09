using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static IdentifierNameExpressionSyntax ParseIdentifierNameExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var identifierToken = iterator.Current.SyntaxKind switch
        {
            SyntaxKind.ThisKeyword => iterator.Match(),
            >= SyntaxKind.AnyKeyword and <= SyntaxKind.F128Keyword => iterator.Match(),
            _ => iterator.Match(SyntaxKind.IdentifierToken)
        };
        return new IdentifierNameExpressionSyntax(syntaxTree, identifierToken);
    }
}
