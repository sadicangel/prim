using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundVariableDeclaration(VariableSymbol Variable, BoundExpression Expression) : BoundDeclaration(Variable, BoundNodeKind.VariableDeclaration)
{
    public override T Accept<T>(IBoundStatementVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
        yield return Expression;
    }
}