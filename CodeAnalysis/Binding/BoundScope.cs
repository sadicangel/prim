using CodeAnalysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed record class BoundScope(BoundScope? Parent, IReadOnlyCollection<Symbol>? Symbols = null)
{
    private Dictionary<string, Symbol>? _symbols = Symbols?.ToDictionary(v => v.Name);

    public IEnumerable<VariableSymbol> Variables { get => _symbols?.Values.OfType<VariableSymbol>() ?? Array.Empty<VariableSymbol>(); }

    public IEnumerable<FunctionSymbol> Functions { get => _symbols?.Values.OfType<FunctionSymbol>() ?? Array.Empty<FunctionSymbol>(); }

    public IEnumerable<TypeSymbol> Types { get => _symbols?.Values.OfType<TypeSymbol>() ?? Array.Empty<TypeSymbol>(); }

    public bool TryDeclare(Symbol symbol) => (_symbols ??= new()).TryAdd(symbol.Name, symbol);

    public bool TryDeclare<T>(T symbol, [MaybeNullWhen(true)] out Symbol existingSymbol) where T : notnull, Symbol
    {
        if ((_symbols ??= new()).TryGetValue(symbol.Name, out existingSymbol))
            return false;

        _symbols[symbol.Name] = symbol;
        return true;
    }

    public bool TryLookup(string name, [MaybeNullWhen(false)] out Symbol symbol)
    {
        symbol = null;

        if (_symbols is not null && _symbols.TryGetValue(name, out symbol))
            return true;

        return Parent is not null && Parent.TryLookup(name, out symbol);
    }

    public bool TryLookup<T>(string name, [NotNullWhen(true)] out T? symbol, [NotNullWhen(true)] out Symbol? existingSymbol) where T : notnull, Symbol
    {
        symbol = null;
        if (TryLookup(name, out existingSymbol) && existingSymbol is T existingSymbolAsT)
            symbol = existingSymbolAsT;
        return symbol is not null;
    }
}
