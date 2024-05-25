using System.Collections;

namespace CodeAnalysis;
public sealed class ReadOnlyList<T> : IList<T>, IReadOnlyList<T>, IEquatable<ReadOnlyList<T>>
{
    private readonly List<T> _values;

    public ReadOnlyList() => _values = [];

    public ReadOnlyList(int capacity) => _values = new(capacity);

    public ReadOnlyList(IEnumerable<T> collection) => _values = new(collection);

    public T this[int index] { get => _values[index]; set => _values[index] = value; }

    public int Count => _values.Count;

    bool ICollection<T>.IsReadOnly => ((ICollection<T>)_values).IsReadOnly;

    public void Add(T item) => _values.Add(item);

    public void Clear() => _values.Clear();

    public bool Contains(T item) => _values.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);

    public bool Equals(ReadOnlyList<T>? other) => other is not null && this.SequenceEqual(other);

    public override bool Equals(object? obj) => Equals(obj as ReadOnlyList<T>);

    public override int GetHashCode() => _values.Aggregate(new HashCode(), (h, v) => { h.Add(v); return h; }, h => h.ToHashCode());

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();

    public int IndexOf(T item) => _values.IndexOf(item);

    public void Insert(int index, T item) => _values.Insert(index, item);

    public bool Remove(T item) => _values.Remove(item);

    public void RemoveAt(int index) => _values.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_values).GetEnumerator();
}
