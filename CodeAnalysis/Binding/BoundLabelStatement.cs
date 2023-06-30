using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundLabelStatement(LabelSymbol Label) : BoundStatement(BoundNodeKind.LabelStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Label;
    }
}