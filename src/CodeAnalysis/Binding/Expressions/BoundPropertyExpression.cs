using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundPropertyExpression(
    SyntaxNode SyntaxNode,
    PropertySymbol Symbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.PropertyExpression, SyntaxNode, Symbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Symbol;
        yield return Expression;
    }
}
