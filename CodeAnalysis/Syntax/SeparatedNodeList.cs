using System.Collections;

namespace CodeAnalysis.Syntax;

public sealed record class SeparatedNodeList<T>(params INode[] Nodes) : IReadOnlyList<T> where T : INode
{
    public int Count { get => (Nodes.Length + 1) / 2; }

    public T this[int index] => (T)Nodes[index * 2];

    public Token GetSeparator(int index) => (Token)Nodes[index * 2 + 1];

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; ++i)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
