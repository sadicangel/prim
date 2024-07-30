﻿using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;
internal sealed record class ModuleSymbol(
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : Symbol(
        BoundKind.ModuleSymbol,
        Syntax,
        Name,
        Predefined.Never,
        ContainingModule,
        IsStatic: true,
        IsReadOnly: true),
    IBoundScope
{
    private readonly Dictionary<string, Symbol> _symbols = [];

    public IBoundScope Parent => ContainingModule;

    public ModuleSymbol Module => this;

    public override IEnumerable<Symbol> DeclaredSymbols => _symbols.Values;
    public bool Declare(Symbol symbol) => _symbols.TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name)
    {
        if (_symbols.TryGetValue(name, out var symbol))
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
        if (_symbols.ContainsKey(symbol.Name))
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
