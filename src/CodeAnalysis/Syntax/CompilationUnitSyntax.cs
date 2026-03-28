using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnitSyntax(
    ModuleDeclarationSyntax? Module,
    SyntaxList<GlobalDeclarationSyntax> Declarations,
    SyntaxToken EofToken)
    : SyntaxNode(SyntaxKind.CompilationUnit)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        if (Module is not null) yield return Module;
        foreach (var declaration in Declarations)
            yield return declaration;
    }
}
