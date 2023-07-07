using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class CompilationUnit(SyntaxTree SyntaxTree, IReadOnlyList<GlobalSyntaxNode> Nodes, Token EOF)
    : SyntaxNode(SyntaxNodeKind.CompilationUnit, SyntaxTree)
{
    public override TextSpan Span { get => Nodes.Count > 0 ? TextSpan.FromBounds(Nodes[0].Span, Nodes[Nodes.Count - 1].Span) : default; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach (var node in Nodes)
            yield return node;
    }
}