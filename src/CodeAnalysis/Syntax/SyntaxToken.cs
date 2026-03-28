using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxToken(
    SyntaxKind SyntaxKind,
    SourceSpan SourceSpan,
    SyntaxList<SyntaxTrivia> LeadingTrivia,
    SyntaxList<SyntaxTrivia> TrailingTrivia,
    object? Value)
    : SyntaxNode(SyntaxKind)
{
    public override SourceSpan SourceSpan { get; } = SourceSpan;

    public override SourceSpan SourceSpanWithTrivia => SourceSpan.Union(
        LeadingTrivia.FirstOrDefault()?.SourceSpanWithTrivia ?? SourceSpan,
        TrailingTrivia.LastOrDefault()?.SourceSpanWithTrivia ?? SourceSpan);

    public override IEnumerable<SyntaxNode> Children() => [];

    public static SyntaxToken CreateSynthetic(SyntaxKind syntaxKind) => new(
        SyntaxKind: syntaxKind,
        SourceSpan: new SourceSpan(SourceText.Empty, default),
        LeadingTrivia: [],
        TrailingTrivia: [],
        Value: null)
    { IsSynthetic = true };
}
