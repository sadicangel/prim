using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static PropertyDeclarationSyntax ParsePropertyDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var name = ParseSimpleName(syntaxTree, iterator);
        var typeClause = ParseTypeClause(syntaxTree, iterator, isOptional: false);
        var initClause = ParseInitClause(syntaxTree, iterator, isOptional: true);
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

        return new PropertyDeclarationSyntax(
            syntaxTree,
            name,
            typeClause,
            initClause,
            semicolonToken);
    }
}
