using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static PropertyDeclarationSyntax ParsePropertyDeclaration(SyntaxIterator iterator)
    {
        var name = ParseSimpleName(iterator);
        var typeClause = ParseTypeClause(iterator, isOptional: false);
        var initClause = ParseInitClause(iterator, isOptional: true);
        var semicolonToken = iterator.Match(SyntaxKind.SemicolonToken);

        return new PropertyDeclarationSyntax(
            name,
            typeClause,
            initClause,
            semicolonToken);
    }
}
