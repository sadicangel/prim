using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static VariableDeclarationSyntax ParseVariableDeclaration(SyntaxIterator iterator)
    {
        var bindingKeyword = iterator.Match(SyntaxKind.LetKeyword, SyntaxKind.VarKeyword);
        var name = ParseSimpleName(iterator);
        var typeClause = ParseTypeClause(iterator, isOptional: true);
        var initClause = ParseInitClause(iterator, isOptional: true);
        _ = iterator.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken);

        return new VariableDeclarationSyntax(
            bindingKeyword,
            name,
            typeClause,
            initClause,
            semicolonToken);
    }
}
