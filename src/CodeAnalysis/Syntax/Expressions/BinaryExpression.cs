using System.Text;

namespace CodeAnalysis.Syntax.Expressions;
public sealed record class BinaryExpression(
    SyntaxTree SyntaxTree,
    Expression Left,
    Token Operator,
    Expression Right,
    Token? OperatorClose = null
)
    : Expression(SyntaxNodeKind.BinaryExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
        if (OperatorClose is not null)
            yield return OperatorClose;
    }

    public override void WriteMarkupTo(StringBuilder builder)
    {
        builder
            .Node(Left)
            .Token(Operator)
            .Node(Right)
            .Token(OperatorClose);
    }
}
