using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public abstract record class SyntaxNode(SyntaxKind SyntaxKind)
{
    public bool IsSynthetic { get; init; }

    public SyntaxToken FirstToken => this as SyntaxToken ?? Children().First().FirstToken;
    public SyntaxToken LastToken => this as SyntaxToken ?? Children().Last().LastToken;

    public virtual SourceSpan SourceSpan => SourceSpan.Union(Children().First().SourceSpan, Children().Last().SourceSpan);
    public virtual SourceSpan SourceSpanWithTrivia => SourceSpan.Union(Children().First(x => !x.IsSynthetic).SourceSpanWithTrivia, Children().Last(x => !x.IsSynthetic).SourceSpanWithTrivia);

    public sealed override string ToString() =>
        SourceSpan.Length > 0 ? $"{SyntaxKind} {SourceSpan}" : SyntaxKind.ToString();

    public abstract IEnumerable<SyntaxNode> Children();
}
