namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTrivia(
    SyntaxKind SyntaxKind,
    SyntaxTree SyntaxTree,
    Range Range)
    : SyntaxNode(SyntaxKind, SyntaxTree)
{
    public static readonly IReadOnlyList<SyntaxTrivia> EmptyList = [];

    public override Range Range { get; } = Range;

    public override Range RangeWithWhiteSpace { get => Range; }

    public override IEnumerable<SyntaxNode> Children() => [];
}