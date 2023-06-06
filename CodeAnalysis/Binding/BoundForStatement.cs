namespace CodeAnalysis.Binding;

internal sealed record class BoundForStatement(Variable Variable, BoundExpression LowerBound, BoundExpression UpperBound, BoundStatement Body) : BoundStatement(BoundNodeKind.ForStatement)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Accept(this);
}
