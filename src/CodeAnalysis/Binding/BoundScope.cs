using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BoundScope
{
    private Dictionary<string, Symbol>? _symbols;

    private BoundScope() { }

    public BoundScope? Parent { get; private init; }

    public bool Declare(Symbol symbol) => (_symbols ??= []).TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name) => _symbols?.GetValueOrDefault(name) ?? Parent?.Lookup(name);

    public static BoundScope From(BoundScope parent) => new() { Parent = parent };

    public static BoundScope Global() => new();

}
