using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundForStatement(VariableSymbol Variable, BoundExpression LowerBound, BoundExpression UpperBound, BoundStatement Body) : BoundStatement(BoundNodeKind.ForStatement)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
        yield return LowerBound;
        yield return UpperBound;
        yield return Body;
    }
}
