using System.Collections;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Syntax;

[CollectionBuilder(typeof(SyntaxListBuilder), nameof(SyntaxListBuilder.Create))]
public readonly struct SyntaxList<T>(List<T> nodes) : IReadOnlyList<T>
    where T : SyntaxNode
{
    private readonly List<T> _nodes = nodes;

    private List<T> Nodes { get => _nodes ?? throw new InvalidOperationException("SyntaxList not initialized"); }

    public T this[int index] => Nodes[index];

    public int Count => Nodes.Count;

    public bool Equals(SyntaxList<T> other)
    {
        if (ReferenceEquals(Nodes, other.Nodes)) return true;
        if (Nodes is null || other.Nodes is null) return false;
        return Nodes.SequenceEqual(other.Nodes);
    }

    public override bool Equals(object? obj) => obj is SyntaxList<T> list && Equals(list);

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

    public static bool operator ==(SyntaxList<T> left, SyntaxList<T> right) => left.Equals(right);

    public static bool operator !=(SyntaxList<T> left, SyntaxList<T> right) => !(left == right);

    public struct Builder : IEnumerable<T>
    {
        private List<T>? _nodes;

        private List<T> Nodes { get => _nodes ??= []; }

        public readonly bool IsDefault => _nodes is null;

        public readonly int Count => _nodes?.Count ?? 0;

        public readonly T? this[int index] => _nodes?[index];

        public void Add(T node) => Nodes.Add(node);

        public void AddRange(IEnumerable<T> nodes) => Nodes.AddRange(nodes);

        public void Remove(T node) => Nodes.Remove(node);

        public SyntaxList<T> ToSyntaxList()
        {
            var list = new SyntaxList<T>(Nodes);
            _nodes = null;
            return list;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_nodes is not null)
                yield break;
            foreach (var node in Nodes)
                yield return node;
        }

        public IEnumerator GetEnumerator() => GetEnumerator();
    }
}

public static class SyntaxListBuilder
{
    public static SyntaxList<T> Create<T>(ReadOnlySpan<T> nodes) where T : SyntaxNode => new([.. nodes]);
}
