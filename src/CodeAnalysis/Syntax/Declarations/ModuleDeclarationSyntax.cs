using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Declarations;

public sealed record class ModuleDeclarationSyntax(
    SyntaxToken ModuleKeyword,
    SimpleNameSyntax Name,
    SyntaxToken SemicolonToken)
    : DeclarationSyntax(SyntaxKind.ModuleDeclaration)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ModuleKeyword;
        yield return Name;
        yield return SemicolonToken;
    }
}
