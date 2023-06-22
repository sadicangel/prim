using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundAssignmentExpression(VariableSymbol Variable, BoundExpression Expression) : BoundExpression(BoundNodeKind.AssignmentExpression, Expression.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
        yield return Expression;
    }
}