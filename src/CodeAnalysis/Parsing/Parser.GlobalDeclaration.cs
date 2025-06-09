using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static GlobalDeclarationSyntax ParseGlobalDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var declaration = ParseDeclaration(syntaxTree, iterator);

        return new GlobalDeclarationSyntax(syntaxTree, declaration);
    }
}
