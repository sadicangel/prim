using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class MemberListExpression(
    SyntaxTree SyntaxTree,
    Token BraceOpen,
    IReadOnlyList<MemberExpression> Members,
    Token BraceClose
)
    : Expression(SyntaxNodeKind.MemberList, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return BraceOpen;
        foreach (var member in Members)
            yield return member;
        yield return BraceClose;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Token(BraceOpen)
            .Nodes(Members)
            .Token(BraceClose);
    }
}
