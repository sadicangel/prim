namespace CodeAnalysis.Syntax;
public sealed record class CompilationUnitSyntax(
    SyntaxTree SyntaxTree,
    IReadOnlyList<SyntaxNode> SyntaxNodes,
    SyntaxToken EofToken
    ) : SyntaxNode(SyntaxKind.CompilationUnit, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        foreach (var syntaxNode in SyntaxNodes)
            yield return syntaxNode;
    }
}
