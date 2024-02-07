
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class InlineExpression(
    SyntaxTree SyntaxTree,
    Expression Expression,
    Token Semicolon
)
    : Expression(SyntaxNodeKind.InlineExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return Semicolon;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Node(Expression)
            .Token(Semicolon);
    }
}
