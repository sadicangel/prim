using CodeAnalysis.Text;
using System.Diagnostics;

namespace CodeAnalysis.Syntax;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class Token(SyntaxTree SyntaxTree, TokenKind TokenKind, int Position, string Text, IReadOnlyList<Trivia> LeadingTrivia, IReadOnlyList<Trivia> TrailingTrivia, object? Value = null)
    : SyntaxNode(SyntaxNodeKind.Token, SyntaxTree)
{
    public bool IsMissing { get => String.IsNullOrEmpty(Text); }

    public override TextSpan Span { get => new(Position, Text.Length); }

    public override TextSpan FullSpan
    {
        get
        {
            var start = LeadingTrivia is [var first, ..] ? first.Span : Span;
            var end = TrailingTrivia is [.., var last] ? last.Span : Span;
            return TextSpan.FromBounds(start, end);
        }
    }

    public override IEnumerable<SyntaxNode> Children() => Enumerable.Empty<SyntaxNode>();

    private string GetDebuggerDisplay() => $"{TokenKind} {{ \"{Value ?? Text}\" }}";
}