
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ForExpression(
    SyntaxTree SyntaxTree,
    Token For,
    Token ParenthesisOpen,
    Token Identifier,
    Token Colon,
    Expression Enumerable,
    Token ParenthesisClose,
    Expression Body
)
    : Expression(SyntaxNodeKind.ForExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return For;
        yield return ParenthesisOpen;
        yield return Identifier;
        yield return Colon;
        yield return Enumerable;
        yield return ParenthesisClose;
        yield return Body;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .ControlFlow(For)
            .Token(ParenthesisOpen)
            .Identifier(Identifier)
            .Token(Colon)
            .Node(Enumerable)
            .Token(ParenthesisClose)
            .Node(Body);
    }
}
