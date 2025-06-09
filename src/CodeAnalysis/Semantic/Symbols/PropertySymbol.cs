using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    TypeSymbol ContainingType,
    Modifiers Modifiers)
    : Symbol(
        SymbolKind.Property,
        Syntax,
        Name,
        Type,
        ContainingType,
        ContainingType.ContainingModule,
        Modifiers)
{
    public bool Equals(PropertySymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
