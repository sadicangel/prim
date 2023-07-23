using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class Trivia(SyntaxTree SyntaxTree, TokenKind TokenKind, int Position, string Text)
    : SyntaxNode(SyntaxNodeKind.Trivia, SyntaxTree)
{
    public override TextSpan Span { get => new(Position, Text.Length); }

    public override TextSpan FullSpan { get => Span; }

    public override IEnumerable<SyntaxNode> Children() => Enumerable.Empty<SyntaxNode>();
}
