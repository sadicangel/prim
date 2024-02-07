using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class ContinueExpression(
    SyntaxTree SyntaxTree,
    Token Continue,
    Expression Result
)
    : Expression(SyntaxNodeKind.ContinueExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Continue;
        yield return Result;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .ControlFlow(Continue)
            .Node(Result);
    }
}