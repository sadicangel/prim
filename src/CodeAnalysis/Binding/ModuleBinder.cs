using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class ModuleBinder(ModuleSymbol module, Binder? parent = null) : Binder(parent)
{
    /// <inheritdoc />
    public override ModuleSymbol Module => module;

    /// <inheritdoc />
    public override bool TryDeclare(Symbol symbol) => module.TryDeclare(symbol) || true; // TODO: Does this make sense?

    /// <inheritdoc />
    protected override bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) =>
        module.TryLookup(name, out symbol);
}
