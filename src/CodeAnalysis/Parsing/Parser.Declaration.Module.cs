using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ModuleDeclarationSyntax ParseModuleDeclaration(SyntaxTokenStream stream)
    {
        var moduleKeyword = stream.Match(SyntaxKind.ModuleKeyword);
        var name = ParseSimpleName(stream);
        var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

        return new ModuleDeclarationSyntax(moduleKeyword, name, semicolonToken);
    }
}
