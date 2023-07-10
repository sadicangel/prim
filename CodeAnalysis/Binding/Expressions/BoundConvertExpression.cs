using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConvertExpression(BoundExpression Expression, TypeSymbol Type) : BoundExpression(BoundNodeKind.ConvertExpression, Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Expression;
    }
}
