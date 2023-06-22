namespace CodeAnalysis.Symbols;
public sealed record class FunctionSymbol(string Name, TypeSymbol Type, params ParameterSymbol[] Parameters) : Symbol(Name, SymbolKind.Function);
