using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class SyntaxTrivia(SyntaxKind Kind, SourceSpan SourceSpan)
    : SyntaxNode(Kind)
{
    public override SourceSpan SourceSpan { get; } = SourceSpan;

    public override SourceSpan SourceSpanWithTrivia { get => SourceSpan; }

    public override IEnumerable<SyntaxNode> Children() => [];
}
