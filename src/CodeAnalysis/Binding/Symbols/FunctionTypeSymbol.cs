namespace CodeAnalysis.Binding.Symbols;

internal sealed record class FunctionTypeSymbol(FunctionSymbol Function) : Symbol(SymbolKind.FunctionType, Function.Name);
