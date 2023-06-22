using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundDeclarationStatement(VariableSymbol Variable, BoundExpression Expression) : BoundStatement(BoundNodeKind.BoundDeclarationStatement)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Accept(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Variable;
        yield return Expression;
    }
}
