namespace CodeAnalysis.Binding;

internal abstract record class BoundStatement(BoundNodeKind Kind) : BoundNode(Kind)
{
    public abstract void Accept(IBoundStatementVisitor visitor);
}
