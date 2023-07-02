namespace CodeAnalysis.Binding;

internal sealed record class BoundReturnStatement(BoundExpression? Expression) : BoundStatement(BoundNodeKind.ReturnStatement)
{
    public BoundReturnStatement() : this(Expression: null) { }
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        if (Expression is not null)
            yield return Expression;
    }
}