using System.Diagnostics;

namespace CodeAnalysis.Syntax;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
public readonly struct SeparatedSyntaxList<T>(SyntaxList<SyntaxNode> syntaxNodes)
    : IEquatable<SeparatedSyntaxList<T>>, IReadOnlyList<T> where T : SyntaxNode
{
    public SyntaxList<SyntaxNode> SyntaxNodes { get; } = syntaxNodes;

    public int Count { get => (SyntaxNodes.Count + 1) / 2; }

    public T this[int index] => (T)SyntaxNodes[index * 2];

    public SyntaxToken GetSeparator(Index index) => (SyntaxToken)SyntaxNodes[index.GetOffset(SyntaxNodes.Count) * 2 + 1];

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; ++i)
            yield return this[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(SeparatedSyntaxList<T> other) => SyntaxNodes == other.SyntaxNodes;

    public override bool Equals(object? obj) => obj is SeparatedSyntaxList<T> list && Equals(list);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var node in SyntaxNodes)
            hash.Add(node);
        return hash.ToHashCode();
    }

    public static bool operator ==(SeparatedSyntaxList<T> left, SeparatedSyntaxList<T> right) => left.Equals(right);

    public static bool operator !=(SeparatedSyntaxList<T> left, SeparatedSyntaxList<T> right) => !(left == right);
}
