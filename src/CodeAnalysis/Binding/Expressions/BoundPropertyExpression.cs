using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundPropertyExpression(
    SyntaxNode SyntaxNode,
    PropertySymbol PropertySymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.PropertyExpression, SyntaxNode, PropertySymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return PropertySymbol;
        yield return Expression;
    }
}
