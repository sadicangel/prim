using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class GroupExpression(
    SyntaxTree SyntaxTree,
    Token ParenthesisOpen,
    Expression Expression,
    Token ParenthesisClose
)
    : Expression(SyntaxNodeKind.GroupExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return ParenthesisOpen;
        yield return Expression;
        yield return ParenthesisClose;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Token(ParenthesisOpen)
            .Node(Expression)
            .Token(ParenthesisClose);
    }
}