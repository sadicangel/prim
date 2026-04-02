using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal sealed record class BoundPropertyReference(SyntaxNode Syntax, PropertySymbol Property)
    : BoundReference(BoundKind.PropertyReference, Syntax, Property)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}