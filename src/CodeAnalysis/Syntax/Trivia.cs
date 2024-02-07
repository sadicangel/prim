using System.Text;

namespace CodeAnalysis.Syntax;

public sealed record class Trivia(SyntaxTree SyntaxTree, TokenKind TokenKind, Range Range)
    : SyntaxNode(SyntaxNodeKind.Trivia, SyntaxTree)
{
    public override Range Range { get; } = Range;

    public override Range RangeWithWhiteSpace { get => Range; }

    public override IEnumerable<SyntaxNode> Children() => Enumerable.Empty<SyntaxNode>();

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder.Append(Text);
    }
}