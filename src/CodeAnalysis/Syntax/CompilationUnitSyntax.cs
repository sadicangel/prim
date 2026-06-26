namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnitSyntax(
    SyntaxList<GlobalDeclarationSyntax> Declarations,
    SyntaxToken EofToken)
    : SyntaxNode(SyntaxKind.CompilationUnit)
{
    public override IEnumerable<SyntaxNode> Children() => Declarations.SyntaxNodes;
}
