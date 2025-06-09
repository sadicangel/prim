using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;
public sealed record class ModuleDeclarationSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken ModuleKeyword,
    SimpleNameSyntax Name,
    SyntaxToken BraceOpenToken,
    SyntaxList<GlobalDeclarationSyntax> Members,
    SyntaxToken BraceCloseToken)
    : DeclarationSyntax(SyntaxKind.ModuleDeclaration, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ModuleKeyword;
        yield return Name;
        yield return BraceOpenToken;
        foreach (var member in Members)
            yield return member;
        yield return BraceCloseToken;
    }
}
