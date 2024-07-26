using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(BoundKind BoundKind, SyntaxNode Syntax)
{
    public virtual bool CanJump() => Children().Any(child => child.CanJump());

    public abstract IEnumerable<BoundNode> Children();
}
