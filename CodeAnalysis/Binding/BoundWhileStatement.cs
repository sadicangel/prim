namespace CodeAnalysis.Binding;

internal sealed record class BoundWhileStatement(BoundExpression Condition, BoundStatement Body) : BoundStatement(BoundNodeKind.WhileStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Condition;
        yield return Body;
    }
}
