using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;
public sealed record class SyntaxToken(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    SourceSpan SourceSpan,
    SyntaxList<SyntaxTrivia> LeadingTrivia,
    SyntaxList<SyntaxTrivia> TrailingTrivia,
    object? Value)
    : SyntaxNode(SyntaxKind, SyntaxTree)
{
    public override SourceSpan SourceSpan { get; } = SourceSpan;

    public override SourceSpan SourceSpanWithTrivia => SourceSpan.Union(
        LeadingTrivia.FirstOrDefault()?.SourceSpanWithTrivia ?? SourceSpan,
        TrailingTrivia.LastOrDefault()?.SourceSpanWithTrivia ?? SourceSpan);

    public override IEnumerable<SyntaxNode> Children() => [];

    public static SyntaxToken CreateSynthetic(SyntaxKind syntaxKind, SyntaxTree? syntaxTree = null, Range range = default, object? value = null) => new(
        SyntaxKind: syntaxKind,
        SyntaxTree: syntaxTree ?? SyntaxTree.Empty,
        SourceSpan: new SourceSpan((syntaxTree ?? SyntaxTree.Empty).SourceText, range),
        LeadingTrivia: [],
        TrailingTrivia: [],
        Value: value)
    {
        IsSynthetic = true
    };
}
