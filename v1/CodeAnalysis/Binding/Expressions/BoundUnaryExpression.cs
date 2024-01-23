using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(SyntaxNode Syntax, BoundUnaryOperator Operator, BoundExpression Operand)
    : BoundExpression(BoundNodeKind.UnaryExpression, Syntax, Operator.ResultType)
{
    public override ConstantValue? ConstantValue { get; } = ConstantFolding.Fold(Operator, Operand);
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children()
    {
        yield return Operand;
    }

    public override string ToString() => base.ToString();
}
