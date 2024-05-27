namespace CodeAnalysis.Syntax;
public sealed record class SyntaxToken(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    Range Range,
    SyntaxList<SyntaxTrivia> LeadingTrivia,
    SyntaxList<SyntaxTrivia> TrailingTrivia,
    object? Value)
    : SyntaxNode(SyntaxKind, SyntaxTree)
{
    public bool IsArtificial { get => SyntaxTree.SourceText[Range].Length == 0; }

    public override Range Range { get; } = Range;

    public override Range RangeWithWhiteSpace
    {
        get
        {
            var start = LeadingTrivia is [var first, ..] ? first.RangeWithWhiteSpace.Start : Range.Start;
            var end = TrailingTrivia is [.., var last] ? last.RangeWithWhiteSpace.End : Range.End;
            return start..end;
        }
    }

    public override IEnumerable<SyntaxNode> Children() => [];
}
