using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal abstract record class BoundNode(BoundKind BoundKind, SyntaxNode Syntax) : ITreeNode
{
    public virtual bool CanExitScope => false;

    public abstract IEnumerable<ITreeNode> Children();
}
