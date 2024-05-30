using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundCompilationUnit(
    SyntaxNode SyntaxNode,
    BoundList<BoundNode> BoundNodes)
    : BoundNode(BoundKind.CompilationUnit, SyntaxNode)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var node in BoundNodes)
            yield return node;
    }
}
