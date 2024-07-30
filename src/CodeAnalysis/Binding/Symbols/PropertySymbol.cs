using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule,
    TypeSymbol ContainingType,
    bool IsReadOnly,
    bool IsStatic)
    : Symbol(
        BoundKind.PropertySymbol,
        Syntax,
        Name,
        Type,
        ContainingModule,
        IsReadOnly,
        IsStatic)
{
    public override IEnumerable<Symbol> DeclaredSymbols => [];


    public bool Equals(PropertySymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
