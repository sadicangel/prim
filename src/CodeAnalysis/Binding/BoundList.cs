using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Binding;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
[CollectionBuilder(typeof(BoundListBuilder), nameof(BoundListBuilder.Create))]
internal readonly struct BoundList<T>(List<T> nodes) : IReadOnlyList<T>
    where T : BoundNode
{
    private readonly List<T> _nodes = nodes;

    private List<T> Nodes { get => _nodes ?? throw new InvalidOperationException("SyntaxList not initialized"); }

    public T this[int index] => Nodes[index];

    public int Count => Nodes.Count;

    public bool Equals(BoundList<T> other)
    {
        if (ReferenceEquals(Nodes, other.Nodes)) return true;
        if (Nodes is null || other.Nodes is null) return false;
        return Nodes.SequenceEqual(other.Nodes);
    }

    public override bool Equals(object? obj) => obj is BoundList<T> list && Equals(list);

    public override int GetHashCode()
    {
        if (Nodes is null) return 0;
        var hash = new HashCode();
        foreach (var node in Nodes)
            hash.Add(node);
        return hash.ToHashCode();
    }

    public IEnumerator<T> GetEnumerator() => Nodes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static bool operator ==(BoundList<T> left, BoundList<T> right) => left.Equals(right);

    public static bool operator !=(BoundList<T> left, BoundList<T> right) => !(left == right);

    public struct Builder(int capacity) : IEnumerable<T>
    {
        private List<T>? _nodes = new(capacity);

        private List<T> Nodes { get => _nodes ??= []; }

        public readonly bool IsDefault => _nodes is null;

        public readonly int Count => _nodes?.Count ?? 0;

        public readonly T? this[int index] => _nodes?[index];

        public void Add(T node) => Nodes.Add(node);

        public void AddRange(IEnumerable<T> nodes) => Nodes.AddRange(nodes);

        public void Remove(T node) => Nodes.Remove(node);

        public BoundList<T> ToBoundList()
        {
            var list = new BoundList<T>(Nodes);
            _nodes = null;
            return list;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_nodes is null)
                yield break;
            foreach (var node in Nodes)
                yield return node;
        }

        public readonly IEnumerator GetEnumerator() => GetEnumerator();
    }
}

internal static class BoundListBuilder
{
    public static BoundList<T> Create<T>(ReadOnlySpan<T> nodes) where T : BoundNode => new([.. nodes]);
}
