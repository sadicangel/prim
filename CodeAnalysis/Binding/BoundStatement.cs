namespace CodeAnalysis.Binding;

internal abstract record class BoundStatement(BoundNodeKind NodeKind) : BoundNode(NodeKind)
{
    public abstract T Accept<T>(IBoundStatementVisitor<T> visitor);
    public override string ToString() => base.ToString();
}
