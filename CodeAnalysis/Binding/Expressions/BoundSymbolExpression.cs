using CodeAnalysis.Symbols;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundSymbolExpression(Symbol Symbol) : BoundExpression(BoundNodeKind.SymbolExpression, Symbol.Type)
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> GetChildren()
    {
        yield return Symbol;
    }
}
