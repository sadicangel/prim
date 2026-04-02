using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class StructTypeSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : TypeSymbol(SymbolKind.Struct, Syntax, Name, ContainingModule)
{
    public bool Equals(StructTypeSymbol? other) => other is not null && SymbolKind == other.SymbolKind && FullName == other.FullName;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, FullName);
}
