using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ModuleDeclarationSyntax ParseModuleDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var moduleKeyword = iterator.Match(SyntaxKind.ModuleKeyword);
        var name = ParseSimpleName(syntaxTree, iterator);
        var braceOpenToken = iterator.Match(SyntaxKind.BraceOpenToken);
        var declarations = ParseSyntaxList(syntaxTree, iterator, [SyntaxKind.BraceCloseToken], ParseGlobalDeclaration);
        var braceCloseToken = iterator.Match(SyntaxKind.BraceCloseToken);

        return new ModuleDeclarationSyntax(
            syntaxTree,
            moduleKeyword,
            name,
            braceOpenToken,
            declarations,
            braceCloseToken);
    }
}
