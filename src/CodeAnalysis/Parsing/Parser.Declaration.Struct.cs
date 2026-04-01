using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static StructDeclarationSyntax ParseStructDeclaration(SyntaxTokenStream stream)
    {
        var structKeyword = stream.Match(SyntaxKind.StructKeyword);
        var name = ParseSimpleName(stream);
        var braceOpenToken = stream.Match(SyntaxKind.BraceOpenToken);
        var properties = ParseSyntaxList(stream, [SyntaxKind.BraceCloseToken], ParsePropertyDeclaration);
        var braceCloseToken = stream.Match(SyntaxKind.BraceCloseToken);

        return new StructDeclarationSyntax(
            structKeyword,
            name,
            braceOpenToken,
            properties,
            braceCloseToken);
    }
}
