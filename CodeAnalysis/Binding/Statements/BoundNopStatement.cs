namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundNopStatement() : BoundStatement(BoundNodeKind.NopStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Enumerable.Empty<INode>();
}
