using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundPropertyExpression(SyntaxNode Syntax, PropertySymbol Property, BoundExpression Value)
    : BoundExpression(BoundKind.PropertyExpression, Syntax, Property.Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Value;
    }
}
