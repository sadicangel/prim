using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundSymbolExpression(Symbol Symbol) : BoundExpression(BoundNodeKind.VariableExpression, Symbol.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Symbol;
    }
}
