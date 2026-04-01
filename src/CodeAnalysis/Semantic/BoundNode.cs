using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal abstract record class BoundNode(BoundKind BoundKind, SyntaxNode Syntax) : ITreeNode
{
    internal BoundNode? Parent { get; private set; }

    public virtual bool CanExitScope => false;

    public abstract IEnumerable<ITreeNode> Children();

    internal void SetParent(BoundNode? parent) => Parent = parent;
}
