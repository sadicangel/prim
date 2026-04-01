using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    public static VariableDeclarationSyntax ParseVariableDeclaration(SyntaxTokenStream stream)
    {
        var bindingKeyword = stream.Match(SyntaxKind.LetKeyword, SyntaxKind.VarKeyword);
        var name = ParseSimpleName(stream);
        var typeClause = ParseTypeClause(stream, isOptional: true);
        var initClause = ParseInitClause(stream, isOptional: true);
        _ = stream.TryMatch(out var semicolonToken, SyntaxKind.SemicolonToken);

        return new VariableDeclarationSyntax(
            bindingKeyword,
            name,
            typeClause,
            initClause,
            semicolonToken);
    }
}
