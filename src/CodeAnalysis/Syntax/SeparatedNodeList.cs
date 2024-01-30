namespace CodeAnalysis.Syntax;
public sealed record class SeparatedNodeList<T>(IReadOnlyList<object> Nodes) : IReadOnlyList<T>
{
    public int Count { get => (Nodes.Count + 1) / 2; }

    public T this[int index] => (T)Nodes[index * 2];

    public Token GetSeparator(Index index) => (Token)Nodes[index.GetOffset(Nodes.Count) * 2 + 1];

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; ++i)
            yield return this[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}