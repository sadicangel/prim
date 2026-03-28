using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static GlobalDeclarationSyntax ParseGlobalDeclaration(SyntaxIterator iterator)
    {
        var declaration = ParseDeclaration(iterator);

        return new GlobalDeclarationSyntax(declaration);
    }
}
