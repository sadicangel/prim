namespace CodeAnalysis.Syntax;
public abstract record class SeparatedSyntaxList<T>(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    SyntaxList<SyntaxNode> SyntaxNodes)
    : SyntaxNode(SyntaxKind, SyntaxTree), IReadOnlyList<T> where T : SyntaxNode
{
    public int Count { get => (SyntaxNodes.Count + 1) / 2; }

    public T this[int index] => (T)SyntaxNodes[index * 2];

    public SyntaxToken GetSeparator(Index index) => (SyntaxToken)SyntaxNodes[index.GetOffset(SyntaxNodes.Count) * 2 + 1];

    public override IEnumerable<SyntaxNode> Children() => SyntaxNodes;

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; ++i)
            yield return this[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
