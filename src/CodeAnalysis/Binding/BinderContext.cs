﻿using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Binding;

internal readonly record struct BinderContext(BoundTree BoundTree, BoundScope BoundScope)
{
    private readonly Stack<BoundScope> _scopes = new([BoundScope]);
    public BoundScope BoundScope { get => _scopes.Peek(); }
    public DiagnosticBag Diagnostics { get => BoundTree.Diagnostics; }

    public TempScope PushScope() => new(this);

    internal readonly ref struct TempScope
    {
        private readonly BinderContext _parent;
        public TempScope(BinderContext parent)
        {
            _parent = parent;
            _parent._scopes.Push(new BoundScope(_parent.BoundScope));
        }
        public void Dispose() => _parent._scopes?.Pop();
    }
}
