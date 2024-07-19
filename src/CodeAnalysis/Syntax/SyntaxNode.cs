using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;
public abstract record class SyntaxNode(SyntaxKind SyntaxKind, SyntaxTree SyntaxTree)
{
    public SyntaxNode? Parent => SyntaxTree.GetParent(this);

    public SyntaxToken FirstToken => this is SyntaxToken token ? token : Children().First().FirstToken;
    public SyntaxToken LastToken => this is SyntaxToken token ? token : Children().Last().LastToken;

    public virtual Range Range => Children().First().Range.Start..Children().Last().Range.End;
    public virtual Range RangeWithTrivia => Children().First().RangeWithTrivia.Start..Children().Last().RangeWithTrivia.End;
    public virtual ReadOnlySpan<char> Text => SyntaxTree.SourceText[Range];
    public virtual ReadOnlySpan<char> TextWithTrivia => SyntaxTree.SourceText[RangeWithTrivia];
    public SourceLocation Location => new(SyntaxTree.SourceText, Range);

    public sealed override string ToString() =>
        SyntaxTree.SourceText[Range] is var sourceText and { Length: > 0 } ? $"{SyntaxKind} {sourceText}" : SyntaxKind.ToString();

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
