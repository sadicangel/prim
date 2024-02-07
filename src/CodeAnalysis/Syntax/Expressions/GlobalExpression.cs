
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class GlobalExpression(
    SyntaxTree SyntaxTree,
    DeclarationExpression Declaration,
    Token Semicolon
)
    : Expression(SyntaxNodeKind.GlobalExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Declaration;
        yield return Semicolon;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Node(Declaration)
            .Token(Semicolon);
    }
}
