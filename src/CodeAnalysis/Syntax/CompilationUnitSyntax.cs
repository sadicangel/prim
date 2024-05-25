namespace CodeAnalysis.Syntax;
public sealed record class CompilationUnitSyntax(
    SyntaxTree SyntaxTree,
    ReadOnlyList<SyntaxNode> SyntaxNodes,
    SyntaxToken EofToken
    ) : SyntaxNode(SyntaxKind.CompilationUnit, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children() => SyntaxNodes;
}
