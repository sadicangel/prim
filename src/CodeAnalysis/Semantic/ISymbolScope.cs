using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Semantic;

internal interface ISymbolScope
{
    bool TryDeclare(Symbol symbol);
    bool TryLookup<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) where TSymbol : Symbol;
}
