using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static TypeDeclarationSyntax ParseTypeDeclaration(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        var identifierToken = iterator.Match(SyntaxKind.IdentifierToken);
        var colonToken = iterator.Match(SyntaxKind.ColonToken);
        var typeToken = iterator.Match(SyntaxKind.TypeKeyword);
        var operatorToken = iterator.Match(SyntaxKind.EqualsToken, SyntaxKind.ColonToken);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var members = ParseSyntaxList(
            syntaxTree,
            iterator,
            [SyntaxKind.BraceCloseToken, SyntaxKind.EofToken],
            ParseMemberDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new TypeDeclarationSyntax(
            syntaxTree,
            identifierToken,
            colonToken,
            typeToken,
            operatorToken,
            braceOpenToken,
            members,
            braceCloseToken);
    }
}
