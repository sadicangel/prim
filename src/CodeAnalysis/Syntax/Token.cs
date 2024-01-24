using System.Diagnostics;

namespace CodeAnalysis.Syntax;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed record class Token(
    SyntaxTree SyntaxTree,
    TokenKind TokenKind,
    Range Range,
    TokenTrivia Trivia,
    object? Value
)
    : SyntaxNode(SyntaxNodeKind.Token, SyntaxTree)
{
    public bool IsMissing { get => SyntaxTree.Source[Range].Length == 0; }

    public override Range Range { get; } = Range;

    public override Range RangeWithWhiteSpace
    {
        get
        {
            var start = Trivia.Leading is [var first, ..] ? first.RangeWithWhiteSpace.Start : Range.Start;
            var end = Trivia.Trailing is [.., var last] ? last.RangeWithWhiteSpace.End : Range.End;
            return start..end;
        }
    }

    public override IEnumerable<SyntaxNode> Children() => Enumerable.Empty<SyntaxNode>();

    private string GetDebuggerDisplay() => $"{TokenKind} {{ \"{Value ?? Text.ToString()}\" }}";
}
