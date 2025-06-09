using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;
public abstract record class SyntaxNode(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
{
    public SyntaxNode? Parent => SyntaxTree.GetParent(this);

    public bool IsSynthetic { get; init; }

    public SyntaxToken FirstToken => this is SyntaxToken token ? token : Children().First().FirstToken;
    public SyntaxToken LastToken => this is SyntaxToken token ? token : Children().Last().LastToken;

    public virtual SourceSpan SourceSpan => SourceSpan.Union(Children().First().SourceSpan, Children().Last().SourceSpan);
    public virtual SourceSpan SourceSpanWithTrivia => SourceSpan.Union(Children().First(x => !x.IsSynthetic).SourceSpanWithTrivia, Children().Last(x => !x.IsSynthetic).SourceSpanWithTrivia);

    public sealed override string ToString() =>
        SourceSpan.Length > 0 ? $"{SyntaxKind} {SourceSpan}" : SyntaxKind.ToString();

    public abstract IEnumerable<SyntaxNode> Children();

    public IEnumerable<SyntaxNode> SelfAndDescendants()
    {
        yield return this;

        var queue = new Queue<SyntaxNode>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            foreach (var child in node.Children())
            {
                yield return child;
                queue.Enqueue(child);
            }
        }
    }

    public IEnumerable<SyntaxNode> Descendants() => SelfAndDescendants().Skip(1);

    public IEnumerable<SyntaxNode> SelfAndAncestors()
    {
        var node = this;
        while (node is not null)
        {
            yield return node;
            node = node.Parent;
        }
    }

    public IEnumerable<SyntaxNode> Ancestors() => SelfAndAncestors().Skip(1);
}
