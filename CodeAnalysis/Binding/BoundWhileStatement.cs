using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundWhileStatement(BoundExpression Condition, BoundStatement Body, LabelSymbol Break, LabelSymbol Continue)
    : BoundLoopBodyStatement(BoundNodeKind.WhileStatement, Break, Continue)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Continue;
        yield return Condition;
        yield return Body;
        yield return Break;
    }
}
