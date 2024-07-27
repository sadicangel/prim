using System.Collections;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Binding;

[CollectionBuilder(typeof(BoundListBuilder), nameof(BoundListBuilder.Create))]
internal readonly record struct BoundList<T>(ImmutableArray<T> BoundNodes) : IReadOnlyList<T>
    where T : BoundNode
{
    public T this[int index] => BoundNodes[index];

    public int Count => BoundNodes.Length;

    public bool Equals(BoundList<T> other) => BoundNodes.SequenceEqual(other.BoundNodes);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var node in BoundNodes)
            hash.Add(node);
        return hash.ToHashCode();
    }

    public IEnumerator<T> GetEnumerator() => ((IReadOnlyList<T>)BoundNodes).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal static class BoundListBuilder
{
    public static BoundList<T> Create<T>(ReadOnlySpan<T> nodes) where T : BoundNode => new([.. nodes]);
}
