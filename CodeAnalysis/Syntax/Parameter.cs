using CodeAnalysis.Text;

namespace CodeAnalysis.Syntax;

public sealed record class Parameter(SyntaxTree SyntaxTree, Token Identifier, Token Colon, Token Type)
    : SyntaxNode(SyntaxNodeKind.Parameter, SyntaxTree)
{
    public override TextSpan Span { get => Identifier.Span; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Identifier;
        yield return Colon;
        yield return Type;
    }
}
