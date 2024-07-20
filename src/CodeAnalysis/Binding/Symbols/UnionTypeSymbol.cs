using CodeAnalysis.Syntax;
using CodeAnalysis.Text;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class UnionTypeSymbol(SyntaxNode Syntax, BoundList<TypeSymbol> Types)
    : TypeSymbol(
        BoundKind.UnionTypeSymbol,
        Syntax,
        string.Join(" | ", Types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)),
        PredefinedSymbols.Type)
{
    public bool Equals(UnionTypeSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    public override bool IsNever => Types.Any(t => t.IsNever);
}
