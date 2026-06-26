using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxToken(
    SyntaxKind Kind,
    SourceSpan SourceSpan,
    SyntaxList<SyntaxTrivia> LeadingTrivia,
    SyntaxList<SyntaxTrivia> TrailingTrivia,
    object? Value)
    : SyntaxNode(Kind)
{
    public override SourceSpan SourceSpan { get; } = SourceSpan;

    public ReadOnlySpan<char> ValueText => Value as string ?? SourceSpan.TextSpan;

    public override SourceSpan SourceSpanWithTrivia => SourceSpan.Union(
        LeadingTrivia.FirstOrDefault()?.SourceSpanWithTrivia ?? SourceSpan,
        TrailingTrivia.LastOrDefault()?.SourceSpanWithTrivia ?? SourceSpan);

    public override IEnumerable<SyntaxNode> Children() => [];

    public static SyntaxToken CreateSynthetic(SyntaxKind syntaxKind, SourceSpan? sourceSpan = null) => new(
        Kind: syntaxKind,
        SourceSpan: sourceSpan ?? new SourceSpan(SourceText.Empty, default),
        LeadingTrivia: [],
        TrailingTrivia: [],
        Value: null)
    { IsSynthetic = true };
}
