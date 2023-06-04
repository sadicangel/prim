namespace CodeAnalysis.Binding;

internal sealed record class BoundDeclarationStatement(Variable Variable, BoundExpression Expression) : BoundStatement(BoundNodeKind.BoundDeclarationStatement)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Accept(this);
}
