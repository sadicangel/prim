
using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class WhileExpression(
    SyntaxTree SyntaxTree,
    Token While,
    Token ParenthesisOpen,
    Expression Condition,
    Token ParenthesisClose,
    Expression Body
)
    : Expression(SyntaxNodeKind.WhileExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return While;
        yield return ParenthesisOpen;
        yield return Condition;
        yield return ParenthesisClose;
        yield return Body;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .ControlFlow(While)
            .Token(ParenthesisOpen)
            .Node(Condition)
            .Append(ParenthesisClose)
            .Node(Body);
    }
}
