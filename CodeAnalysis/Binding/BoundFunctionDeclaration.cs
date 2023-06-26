using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundFunctionDeclaration(FunctionSymbol Function, BoundStatement Body) : BoundDeclaration(Function)
{
    public override void Accept(IBoundStatementVisitor visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Function;
        yield return Body;
    }
}