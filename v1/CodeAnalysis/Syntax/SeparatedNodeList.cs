using System.Collections;

namespace CodeAnalysis.Syntax;

public sealed record class SeparatedNodeList<T>(IReadOnlyList<SyntaxNode> Nodes) : IReadOnlyList<T> where T : SyntaxNode
{
    public int Count { get => (Nodes.Count + 1) / 2; }

    public T this[int index] => (T)Nodes[index * 2];

    public Token GetSeparator(Index index) => (Token)Nodes[index.GetOffset(Nodes.Count) * 2 + 1];

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; ++i)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
