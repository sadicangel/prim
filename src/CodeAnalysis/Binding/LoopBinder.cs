using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class LoopBinder(Binder parent) : Binder(parent)
{
    /// <inheritdoc />
    public override ModuleSymbol Module => Parent!.Module;

    /// <inheritdoc />
    public override bool TryDeclare(Symbol symbol) => Parent!.TryDeclare(symbol);

    /// <inheritdoc />
    protected override bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol)
    {
        symbol = null;
        return false;
    }
}
