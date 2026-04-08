using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class PointerTypeSymbol(SyntaxNode Syntax, TypeSymbol ElementType, ModuleSymbol ContainingModule)
    : TypeSymbol(SymbolKind.Pointer, Syntax, $"{ElementType.Name}{SyntaxFacts.GetText(SyntaxKind.AsteriskToken)}", ContainingModule)
{
    public bool Equals(PointerTypeSymbol? other) => other is not null && SymbolKind == other.SymbolKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, Name);
}
