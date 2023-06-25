using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class Token(TokenKind Kind, int Position, string Text, object? Value = null) : Node(NodeKind.Token)
{
    public bool IsMissing { get => String.IsNullOrEmpty(Text); }

    public override TextSpan Span { get => new(Position, Text.Length); }

    public override IEnumerable<Node> GetChildren() => Enumerable.Empty<Node>();
}