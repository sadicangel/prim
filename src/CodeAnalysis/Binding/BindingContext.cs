using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Binding;

internal readonly record struct BindingContext(BoundTree BoundTree, BoundScope BoundScope)
{
    private readonly Stack<BoundScope> _scopes = new([BoundScope]);
    public BoundScope BoundScope { get => _scopes.Peek(); }
    public DiagnosticBag Diagnostics { get => BoundTree.Diagnostics; }
}
