using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundVariableExpression(VariableSymbol Variable) : BoundExpression(BoundNodeKind.VariableExpression, Variable.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
    }
}
