using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal abstract record class ContainerSymbol(
    SymbolKind SymbolKind,
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    Symbol ContainingSymbol,
    ModuleSymbol ContainingModule,
    Modifiers Modifiers)
    : Symbol(SymbolKind, Syntax, Name, Type, ContainingSymbol, ContainingModule, Modifiers), ISymbolScope
{

    private Dictionary<string, Symbol>? _members = [];

    public IEnumerable<Symbol> Members => _members?.Values.AsEnumerable() ?? [];

    public bool TryDeclare(Symbol symbol) => (_members ??= []).TryAdd(symbol.Name, symbol);

    public bool TryLookup(string name, [MaybeNullWhen(false)] out Symbol symbol)
    {
        symbol = null;

        // TODO: Handle qualified names.
        return _members?.TryGetValue(name, out symbol) is true
            || !ReferenceEquals(this, ContainingModule) && ContainingModule.TryLookup(name, out symbol);
    }

    public bool TryLookup<T>(string name, [MaybeNullWhen(false)] out T symbol) where T : Symbol
    {
        if (!TryLookup(name, out var result))
        {
            symbol = default;
            return false;
        }

        symbol = result as T;
        return symbol is not null;
    }
}
