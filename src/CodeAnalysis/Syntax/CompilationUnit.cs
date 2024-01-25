namespace CodeAnalysis.Syntax;
public sealed record class CompilationUnit(SyntaxTree SyntaxTree, IReadOnlyList<SyntaxNode> Nodes, Token EofToken)
    : SyntaxNode(SyntaxNodeKind.CompilationUnit, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        foreach (var node in Nodes)
            yield return node;
        yield return EofToken;
    }
}