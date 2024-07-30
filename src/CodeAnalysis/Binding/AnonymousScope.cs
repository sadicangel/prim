using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class AnonymousScope(IBoundScope parent) : IBoundScope
{
    private Dictionary<string, Symbol>? _symbols;

    public IBoundScope Parent => parent;
    public ModuleSymbol Module => parent.Module;

    public bool Declare(Symbol symbol) => (_symbols ??= []).TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name)
    {
        if (_symbols?.TryGetValue(name, out var symbol) is true)
        {
            return symbol;
        }

        if (!ReferenceEquals(this, Parent))
        {
            return Parent.Lookup(name);
        }

        return null;
    }

    public bool Replace(Symbol symbol)
    {
        if (_symbols?.ContainsKey(symbol.Name) is true)
        {
            _symbols[symbol.Name] = symbol;
            return true;
        }

        if (!ReferenceEquals(this, Parent))
        {
            return Parent.Replace(symbol);
        }

        return false;
    }
}
