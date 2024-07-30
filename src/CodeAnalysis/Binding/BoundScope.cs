using System.Collections;
using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Binding;

internal class BoundScope(BoundScope? parent = null) : IEnumerable<Symbol>
{
    protected Dictionary<string, Symbol>? Symbols { get; set; }

    public BoundScope Parent { get => parent ?? GlobalBoundScope.Instance; }

    public bool Declare(Symbol symbol) => (Symbols ??= []).TryAdd(symbol.Name, symbol);

    public void Replace(Symbol symbol)
    {
        var scope = this;
        if (scope.Symbols?.ContainsKey(symbol.Name) is true)
        {
            scope.Symbols[symbol.Name] = symbol;
            return;
        }

        do
        {
            scope = scope.Parent;
            if (scope.Symbols?.ContainsKey(symbol.Name) is true)
            {
                scope.Symbols[symbol.Name] = symbol;
                return;
            }
        }
        while (scope != scope.Parent);

        throw new UnreachableException(DiagnosticMessage.UndefinedSymbol(symbol.Name));
    }

    public Symbol? Lookup(string name)
    {
        var scope = this;
        var symbol = scope.Symbols?.GetValueOrDefault(name);
        if (symbol is not null)
            return symbol;

        do
        {
            scope = scope.Parent;
            symbol = scope.Symbols?.GetValueOrDefault(name);
            if (symbol is not null)
                return symbol;
        }
        while (scope != scope.Parent);

        return null;
    }

    public IEnumerator<Symbol> GetEnumerator()
    {
        foreach (var symbol in EnumerateSymbols(this))
            yield return symbol;

        static IEnumerable<Symbol> EnumerateSymbols(BoundScope? scope)
        {
            if (scope is null) yield break;
            if (scope.Symbols is not null)
            {
                foreach (var (_, symbol) in scope.Symbols)
                    yield return symbol;
            }
            foreach (var symbol in EnumerateSymbols(scope.Parent))
                yield return symbol;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
