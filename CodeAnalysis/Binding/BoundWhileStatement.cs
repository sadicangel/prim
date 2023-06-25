namespace CodeAnalysis.Binding;

internal sealed record class BoundWhileStatement(BoundExpression Condition, BoundStatement Body) : BoundStatement(BoundNodeKind.WhileStatement)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Condition;
        yield return Body;
    }
}
