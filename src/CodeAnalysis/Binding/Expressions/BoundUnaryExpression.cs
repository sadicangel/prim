using CodeAnalysis.Operators;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    UnaryOperator Operator,
    BoundExpression Operand
)
    : BoundExpression(BoundNodeKind.UnaryExpression, Syntax, Operator.ResultType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Operand;
    }

    public override string ToString() => base.ToString();
}
