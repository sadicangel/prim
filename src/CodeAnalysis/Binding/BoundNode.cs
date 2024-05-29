using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding;

internal abstract record class BoundNode(BoundKind BoundKind, SyntaxNode SyntaxNode)
{
    public abstract IEnumerable<BoundNode> Children();
}
