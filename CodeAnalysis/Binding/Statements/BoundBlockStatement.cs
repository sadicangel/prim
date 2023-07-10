namespace CodeAnalysis.Binding.Statements;

internal sealed record class BoundBlockStatement(IReadOnlyList<BoundStatement> Statements) : BoundStatement(BoundNodeKind.BlockStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren() => Statements;
    public override string ToString() => base.ToString();
}