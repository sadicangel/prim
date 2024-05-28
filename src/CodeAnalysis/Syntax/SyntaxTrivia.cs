namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTrivia(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    Range Range)
    : SyntaxNode(SyntaxKind, SyntaxTree)
{
    public override Range Range { get; } = Range;

    public override Range RangeWithTrivia { get => Range; }

    public override IEnumerable<SyntaxNode> Children() => [];
}