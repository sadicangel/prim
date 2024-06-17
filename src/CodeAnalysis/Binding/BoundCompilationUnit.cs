using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal sealed record class BoundCompilationUnit(
    SyntaxNode Syntax,
    BoundList<BoundNode> BoundNodes)
    : BoundNode(BoundKind.CompilationUnit, Syntax)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var node in BoundNodes)
            yield return node;
    }
}
