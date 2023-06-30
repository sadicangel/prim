namespace CodeAnalysis.Binding;

internal sealed record class BoundContinueStatement() : BoundStatement(BoundNodeKind.ContinueStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();
}