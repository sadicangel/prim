using System.Diagnostics;
using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundScope
{
    private Dictionary<string, Symbol>? _symbols;

    private BoundScope() { }

    public BoundScope? Parent { get; private init; }

    public bool IsGlobal { get => Parent is null; }

    public bool Declare(Symbol symbol) => (_symbols ??= []).TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name) => _symbols?.GetValueOrDefault(name) ?? Parent?.Lookup(name);

    public void Replace(Symbol symbol)
    {
        var scope = this;
        while (scope is not null)
        {
            if (scope._symbols is not null && scope._symbols.ContainsKey(symbol.Name))
            {
                scope._symbols[symbol.Name] = symbol;
                return;
            }
            scope = scope.Parent;
        }
        throw new UnreachableException($"Unexpected {nameof(Symbol)} '{symbol}'");
    }

    public static BoundScope ChildOf(BoundScope parent) => new() { Parent = parent };

    public static BoundScope Global() => new();

}
