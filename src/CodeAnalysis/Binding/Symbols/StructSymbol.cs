namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructSymbol(string Name) : Symbol(SymbolKind.Struct, Name);
