using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    public static VariableDeclarationSyntax ParseVariableDeclaration(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        var varOrLetKeyword = iterator.Match(SyntaxKind.LetKeyword, SyntaxKind.VarKeyword);
        var name = ParseSimpleName(syntaxTree, iterator);
        var typeClause = ParseTypeClause(syntaxTree, iterator, isOptional: true);
        var initClause = ParseInitClause(syntaxTree, iterator, isOptional: true);
        _ = iterator.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken);

        return new VariableDeclarationSyntax(
            syntaxTree,
            varOrLetKeyword,
            name,
            typeClause,
            initClause,
            semicolonToken);
    }
}
