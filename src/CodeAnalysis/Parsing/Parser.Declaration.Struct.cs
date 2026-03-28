using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static StructDeclarationSyntax ParseStructDeclaration(SyntaxIterator iterator)
    {
        var structKeyword = iterator.Match(SyntaxKind.StructKeyword);
        var name = ParseSimpleName(iterator);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var properties = ParseSyntaxList(iterator, [SyntaxKind.BraceCloseToken], ParsePropertyDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new StructDeclarationSyntax(
            structKeyword,
            name,
            braceOpenToken,
            properties,
            braceCloseToken);
    }
}
