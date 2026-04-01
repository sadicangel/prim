using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic.Declarations;

internal sealed record class BoundStructDeclaration(
    StructTypeSymbol StructSymbol,
    ImmutableArray<BoundPropertyDeclaration> Properties)
    : BoundDeclaration(BoundKind.StructDeclaration, StructSymbol.Syntax, StructSymbol.ContainingModule.RuntimeType)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children() => Properties;
}
