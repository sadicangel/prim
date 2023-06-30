namespace CodeAnalysis.Binding;

internal sealed record class BoundBreakStatement() : BoundStatement(BoundNodeKind.BreakStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();
}