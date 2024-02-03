﻿using CodeAnalysis.Types;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CodeAnalysis.Binding;

internal sealed record class Scope : IEnumerable<Symbol>
{
    private Dictionary<string, Symbol>? _symbols;

    private Scope() { }

    public Scope? Parent { get; init; }

    public bool IsGlobal { get => Parent is null; }

    public Scope CreateChildScope() => new() { Parent = this };

    public void Declare(Symbol symbol)
    {
        if (!TryDeclare(symbol))
            throw new InvalidOperationException($"Unexpected redeclaration of symbol {symbol}");
    }

    public bool TryDeclare(Symbol symbol) =>
        (_symbols ??= []).TryAdd(symbol.Name, symbol);

    public Symbol? Lookup(string name) =>
        _symbols?.GetValueOrDefault(name) ?? Parent?.Lookup(name);

    public bool TryLookup(string name, [MaybeNullWhen(false)] out Symbol symbol) =>
        _symbols?.TryGetValue(name, out symbol) ?? Parent?.TryLookup(name, out symbol) ?? (symbol = null) is not null;

    public IEnumerator<Symbol> GetEnumerator() => _symbols?.Values.GetEnumerator() ?? Enumerable.Empty<Symbol>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Scope CreateGlobalScope()
    {
        var scope = new Scope();

        foreach (var type in PredefinedTypes.All)
            scope.Declare(new Symbol(type.Name, new TypeType(type)));

        return scope;
    }
}

// TODO: Use ReadOnlyMemory<char> instead os strings?
file sealed class SymbolDictionaryComparer : IEqualityComparer<ReadOnlyMemory<char>>
{
    public static readonly SymbolDictionaryComparer Default = new();
    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => x.Span.Equals(y.Span, StringComparison.Ordinal);
    public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj) => string.GetHashCode(obj.Span, StringComparison.Ordinal);
}