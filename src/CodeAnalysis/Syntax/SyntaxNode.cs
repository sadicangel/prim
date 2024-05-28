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
}
