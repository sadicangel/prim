using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundLabelDeclaration(LabelSymbol Label) : BoundDeclaration(BoundNodeKind.LabelStatement, Label)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Label;
    }
}