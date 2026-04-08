using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.References;

internal sealed record class BoundConversionReference(
    SyntaxNode Syntax,
    ConversionSymbol Conversion)
    : BoundReference(BoundKind.ConversionReference, Syntax, Conversion)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => [];
}
