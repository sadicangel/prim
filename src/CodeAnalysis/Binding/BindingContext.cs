using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Diagnostics;
using CodeAnalysis.Semantic;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal sealed record class BindingContext(ModuleSymbol Module, DiagnosticBag Diagnostics)
{
    private readonly Stack<ISymbolScope> _scopes = new([Module]);

    internal ISymbolScope CurrentScope => _scopes.Peek();

    public bool TryDeclare(Symbol symbol) =>
        CurrentScope.TryDeclare(symbol);

    public bool TryLookup(string name, [MaybeNullWhen(false)] out Symbol symbol) =>
        CurrentScope.TryLookup(name, out symbol);

    public bool TryLookup<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) where TSymbol : Symbol =>
        CurrentScope.TryLookup(name, out symbol);

    public IDisposable PushScope() => new AnonymousScope(this);

    private sealed class AnonymousScope : ISymbolScope, IDisposable
    {
        private readonly BindingContext _context;
        private readonly ISymbolScope? _parent;

        private Dictionary<string, Symbol>? _members = [];

        public IEnumerable<Symbol> Members => _members?.Values.AsEnumerable() ?? [];

        public AnonymousScope(BindingContext context, ISymbolScope? parent = null)
        {
            _context = context;
            _context._scopes.Push(this);
            _parent = parent;
        }

        public void Dispose() => _context._scopes.Pop();

        public bool TryDeclare(Symbol symbol) => (_members ??= []).TryAdd(symbol.Name, symbol);

        public bool TryLookup(string name, [MaybeNullWhen(false)] out Symbol symbol)
        {
            symbol = default;

            return _members?.TryGetValue(name, out symbol) is true
                || !ReferenceEquals(this, _parent) && _parent is not null && _parent.TryLookup(name, out symbol);
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
}
