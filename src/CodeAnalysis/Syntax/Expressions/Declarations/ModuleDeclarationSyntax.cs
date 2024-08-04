using CodeAnalysis.Syntax.Expressions.Names;

namespace CodeAnalysis.Syntax.Expressions.Declarations;
public sealed record class ModuleDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SimpleNameSyntax Name,
    SyntaxToken ColonToken,
    SyntaxToken ModuleKeyword,
    SyntaxToken EqualsToken,
    SyntaxToken BraceOpenToken,
    SyntaxList<DeclarationSyntax> Declarations,
    SyntaxToken BraceCloseToken
    )
    : DeclarationSyntax(SyntaxKind.ModuleDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Name;
        yield return ColonToken;
        yield return ModuleKeyword;
        yield return EqualsToken;
        yield return BraceOpenToken;
        foreach (var declaration in Declarations)
            yield return declaration;
        yield return BraceCloseToken;
    }
}
