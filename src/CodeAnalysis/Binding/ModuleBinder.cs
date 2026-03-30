using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class ModuleBinder(ModuleSymbol module, Binder? parent = null) : Binder(parent)
{
    /// <inheritdoc />
    public override ModuleSymbol Module => module;

    /// <inheritdoc />
    public override bool TryDeclare(Symbol symbol)
    {
        // The compiler guarantees that the symbol is already declared on the previous pass.
        Debug.Assert(TryLookupInCurrentScope<Symbol>(symbol.Name, out _));
        return true;
    }

    /// <inheritdoc />
    protected override bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) =>
        module.TryLookup(name, out symbol);
}
