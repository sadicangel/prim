using System.Collections;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Syntax;

[CollectionBuilder(typeof(SyntaxListBuilder), nameof(SyntaxListBuilder.Create))]
public sealed class SyntaxList<T>(List<T> values) : IReadOnlyList<T>
    where T : SyntaxNode
{
    private readonly List<T> _values = values;

    public T this[int index] => _values[index];

    public int Count => _values.Count;

    public bool Equals(SyntaxList<T>? other) => other is not null && (ReferenceEquals(this, other) || this.SequenceEqual(other));

    public override bool Equals(object? obj) => Equals(obj as SyntaxList<T>);

    public override int GetHashCode() => _values.Aggregate(new HashCode(), (h, v) => { h.Add(v); return h; }, h => h.ToHashCode());

    public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Builder
    {
        private List<T>? _values;

        private List<T> Values { get => _values ??= []; }

        public readonly bool IsDefault => _values is null;

        public readonly int Count => _values?.Count ?? 0;

        public readonly T? this[int index] => _values?[index];

        public void Add(T item) => Values.Add(item);

        public void AddRange(IEnumerable<T> item) => Values.AddRange(item);

        public void Remove(T item) => Values.Remove(item);

        public SyntaxList<T> ToSyntaxList()
        {
            var list = new SyntaxList<T>(Values);
            _values = null;
            return list;
        }
    }
}

public static class SyntaxListBuilder
{
    public static SyntaxList<T> Create<T>(ReadOnlySpan<T> values) where T : SyntaxNode => new([.. values]);
}
