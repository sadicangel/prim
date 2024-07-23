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
    public bool IsSynthetic { get => SyntaxTree.SourceText[Range].Length == 0; }

    public override Range Range { get; } = Range;

    public override Range RangeWithTrivia
    {
        get
        {
            var start = LeadingTrivia is [var first, ..] ? first.RangeWithTrivia.Start : Range.Start;
            var end = TrailingTrivia is [.., var last] ? last.RangeWithTrivia.End : Range.End;
            return start..end;
        }
    }

    public override IEnumerable<SyntaxNode> Children() => [];
}
