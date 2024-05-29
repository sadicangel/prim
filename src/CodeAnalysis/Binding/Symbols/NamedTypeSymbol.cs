namespace CodeAnalysis.Binding.Symbols;
internal sealed record class NamedTypeSymbol(StructSymbol Struct) : Symbol(SymbolKind.NamedType, Struct.Name);
