using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal abstract record class BoundNode(BoundKind BoundKind, SyntaxNode Syntax)
{
    public virtual bool CanExitScope => false;

    public abstract IEnumerable<BoundNode> Children();
}
