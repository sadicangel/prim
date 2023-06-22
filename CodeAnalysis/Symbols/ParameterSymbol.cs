namespace CodeAnalysis.Symbols;

public sealed record class ParameterSymbol(string Name, TypeSymbol Type) : Symbol(Name, SymbolKind.Parameter);