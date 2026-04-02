using System.Collections.Immutable;
using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class UnionTypeSymbol(SyntaxNode Syntax, ImmutableArray<TypeSymbol> Types, ModuleSymbol ContainingModule)
    : TypeSymbol(
        SymbolKind.Union,
        Syntax,
        string.Join(" | ", Types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)),
        ContainingModule)
{
    public bool Equals(UnionTypeSymbol? other) => other is not null && SymbolKind == other.SymbolKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, Name);
}
