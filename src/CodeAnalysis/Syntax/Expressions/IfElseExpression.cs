
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class IfElseExpression(
    SyntaxTree SyntaxTree,
    Token If,
    Token ParenthesisOpen,
    Expression Condition,
    Token ParenthesisClose,
    Expression Then,
    Token ElseToken,
    Expression Else
)
    : Expression(SyntaxNodeKind.IfElseExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return If;
        yield return ParenthesisOpen;
        yield return Condition;
        yield return ParenthesisClose;
        yield return Then;
        yield return ElseToken;
        yield return Else;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .ControlFlow(If)
            .Token(ParenthesisOpen)
            .Node(Condition)
            .Token(ParenthesisClose)
            .Node(Then)
            .Token(ElseToken)
            .Node(Else);
    }
}
