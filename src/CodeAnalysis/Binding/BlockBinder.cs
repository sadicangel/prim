using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class BlockBinder(Binder parent) : Binder(parent)
{
    private Dictionary<string, Symbol>? _locals;

    /// <inheritdoc />
    public override ModuleSymbol Module => Parent!.Module;

    /// <inheritdoc />
    public override bool TryDeclare(Symbol symbol)
    {
        if (symbol is not VariableSymbol variable)
        {
            // TODO: We need to report this as invalid?
            return false;
        }

        return (_locals ??= []).TryAdd(variable.Name, variable);
    }

    /// <inheritdoc />
    protected override bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol)
    {
        // TODO: Handle qualified names.
        if (_locals?.TryGetValue(name, out var result) is not true)
        {
            symbol = null;
            return false;
        }

        symbol = result as TSymbol;
        return symbol is not null;
    }
}
