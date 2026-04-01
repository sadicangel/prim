using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic.Declarations;

internal sealed record class BoundPropertyDeclaration(PropertySymbol Property, BoundExpression? Initializer)
    : BoundDeclaration(BoundKind.PropertyDeclaration, Property.Syntax, Property.Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        if (Initializer is not null)
            yield return Initializer;
    }
}
