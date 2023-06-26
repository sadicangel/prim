using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class Parameter(Token Identifier, Token Colon, Token Type) : SyntaxNode(SyntaxNodeKind.Parameter)
{
    public override TextSpan Span { get => Identifier.Span; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return Colon;
        yield return Type;
    }
}
