using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static StructDeclarationSyntax ParseStructDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var structKeyword = iterator.Match(SyntaxKind.StructKeyword);
        var name = ParseSimpleName(syntaxTree, iterator);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var members = ParseSyntaxList(syntaxTree, iterator, [SyntaxKind.BraceCloseToken], ParsePropertyDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new StructDeclarationSyntax(
            syntaxTree,
            structKeyword,
            name,
            braceOpenToken,
            members,
            braceCloseToken);
    }
}
