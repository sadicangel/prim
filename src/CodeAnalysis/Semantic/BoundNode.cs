using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic;

internal abstract record class BoundNode(BoundKind BoundKind, SyntaxNode Syntax)
{
    public virtual bool CausesScopeExit => Children().Any(child => child.CausesScopeExit);

    public abstract IEnumerable<BoundNode> Children();
}
