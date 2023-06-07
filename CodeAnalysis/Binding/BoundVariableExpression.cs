namespace CodeAnalysis.Binding;

internal sealed record class BoundVariableExpression(Variable Variable) : BoundExpression(BoundNodeKind.VariableExpression, Variable.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
    }
}
