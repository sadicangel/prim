namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionSymbol(string Name) : Symbol(SymbolKind.Function, Name);
