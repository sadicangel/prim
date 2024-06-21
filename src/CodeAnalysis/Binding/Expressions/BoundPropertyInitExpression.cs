using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundPropertyInitExpression(
    SyntaxNode Syntax,
    PropertySymbol PropertySymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.PropertyInitExpression, Syntax, PropertySymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return PropertySymbol;
        yield return Expression;
    }
}
