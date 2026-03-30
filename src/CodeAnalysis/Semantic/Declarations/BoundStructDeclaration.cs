using System.Collections.Immutable;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic.Declarations;

internal sealed record class BoundStructDeclaration(
    StructSymbol StructSymbol,
    ImmutableArray<BoundPropertyDeclaration> Properties)
    : BoundDeclaration(BoundKind.StructDeclaration, StructSymbol.Syntax, StructSymbol.ContainingModule.RuntimeType)
{
    /// <inheritdoc />
    public override IEnumerable<ITreeNode> Children()
    {
        yield return StructSymbol;
        foreach (var property in Properties)
            yield return property;
    }
}
