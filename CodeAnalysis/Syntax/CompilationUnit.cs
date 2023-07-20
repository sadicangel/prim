namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnit(SyntaxTree SyntaxTree, IReadOnlyList<GlobalSyntaxNode> Nodes, Token EOF)
    : SyntaxNode(SyntaxNodeKind.CompilationUnit, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach (var node in Nodes)
            yield return node;
        yield return EOF;
    }
}