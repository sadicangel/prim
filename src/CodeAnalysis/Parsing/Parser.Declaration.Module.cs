using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ModuleDeclarationSyntax ParseModuleDeclaration(SyntaxIterator iterator)
    {
        var moduleKeyword = iterator.Match(SyntaxKind.ModuleKeyword);
        var name = ParseSimpleName(iterator);
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

        return new ModuleDeclarationSyntax(moduleKeyword, name, semicolonToken);
    }
}
