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
    private Dictionary<string, Symbol>? _members;

    public IEnumerable<Symbol> Members => _members?.Values.AsEnumerable() ?? [];

    public bool TryDeclare(Symbol symbol) => (_members ??= []).TryAdd(symbol.Name, symbol);

    public bool TryLookup<T>(string name, [MaybeNullWhen(false)] out T symbol) where T : Symbol
    {
        // TODO: Handle qualified names.
        if (_members?.TryGetValue(name, out var result) is not true)
        {
            symbol = null;
            return false;
        }

        symbol = result as T;
        return symbol is not null;
    }
}
