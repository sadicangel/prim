using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundIfExpression(BoundExpression Condition, BoundExpression Then, BoundExpression Else, TypeSymbol Type) : BoundExpression(BoundNodeKind.IfExpression, Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Condition;
        yield return Then;
        yield return Else;
    }
}