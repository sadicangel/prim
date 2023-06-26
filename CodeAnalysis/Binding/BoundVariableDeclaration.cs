using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundVariableDeclaration(VariableSymbol Variable, BoundExpression Expression) : BoundDeclaration(Variable)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
        yield return Expression;
    }
}