using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundConditionalGotoStatement(LabelSymbol Label, BoundExpression Condition, bool JumpIfTrue = true) : BoundStatement(BoundNodeKind.ConditionalGotoStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Label;
    }
}
