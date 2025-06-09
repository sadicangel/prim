using CodeAnalysis.Syntax.Declarations;

namespace CodeAnalysis.Syntax;
public sealed record class CompilationUnitSyntax(
    SyntaxTree SyntaxTree,
    SyntaxList<GlobalDeclarationSyntax> Declarations,
    SyntaxToken EofToken
    ) : SyntaxNode(SyntaxKind.CompilationUnit, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children() => Declarations;
}
