using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class UnaryExpression(
    SyntaxTree SyntaxTree,
    Token Operator,
    Expression Operand
)
    : Expression(SyntaxNodeKind.BinaryExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Operator;
        yield return Operand;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Token(Operator)
            .Node(Operand);
    }
}
