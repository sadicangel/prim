namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(string Name, bool IsReadOnly) : Symbol(SymbolKind.Variable, Name);
