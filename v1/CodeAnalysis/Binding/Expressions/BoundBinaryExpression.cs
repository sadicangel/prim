using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(SyntaxNode Syntax, BoundExpression Left, BoundBinaryOperator Operator, BoundExpression Right)
    : BoundExpression(BoundNodeKind.BinaryExpression, Syntax, Operator.ResultType)
{
    public override ConstantValue? ConstantValue { get; } = ConstantFolding.Fold(Left, Operator, Right);
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Children()
    {
        yield return Left;
        yield return Right;
    }
    public override string ToString() => base.ToString();
}