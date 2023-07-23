using CodeAnalysis.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundSymbolExpression(SyntaxNode Syntax, Symbol Symbol)
    : BoundExpression(BoundNodeKind.SymbolExpression, Syntax, Symbol.Type)
{
    public override ConstantValue? ConstantValue { get; } = (Symbol as VariableSymbol)?.Constant;
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
    public override IEnumerable<INode> Descendants()
    {
        yield return Symbol;
    }
}
