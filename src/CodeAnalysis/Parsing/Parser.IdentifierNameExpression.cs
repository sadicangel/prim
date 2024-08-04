using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static SimpleNameExpressionSyntax ParseSimpleNameExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var identifierToken = iterator.Current.SyntaxKind switch
        {
            SyntaxKind.ThisKeyword => iterator.Match(),
            >= SyntaxKind.AnyKeyword and <= SyntaxKind.F128Keyword => iterator.Match(),
            _ => iterator.Match(SyntaxKind.IdentifierToken)
        };
        return new SimpleNameExpressionSyntax(syntaxTree, identifierToken);
    }
}
