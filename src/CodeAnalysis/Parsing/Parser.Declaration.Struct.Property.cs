using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static PropertyDeclarationSyntax ParsePropertyDeclaration(SyntaxTokenStream stream)
    {
        var name = ParseSimpleName(stream);
        var typeClause = ParseTypeClause(stream, isOptional: false);
        var initClause = ParseInitClause(stream, isOptional: true);
        var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

        return new PropertyDeclarationSyntax(
            name,
            typeClause,
            initClause,
            semicolonToken);
    }
}
