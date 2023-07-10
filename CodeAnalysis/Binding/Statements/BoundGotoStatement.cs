using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Statements;
internal sealed record class BoundGotoStatement(LabelSymbol Label) : BoundStatement(BoundNodeKind.GotoStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Label;
    }
}
