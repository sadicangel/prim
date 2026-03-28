using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Semantic.Symbols;

namespace CodeAnalysis.Binding;

internal sealed class TypeBinder(TypeSymbol type, Binder? parent = null) : Binder(parent)
{
    /// <inheritdoc />
    public override ModuleSymbol Module => type.ContainingModule;

    public TypeSymbol Type => type;

    /// <inheritdoc />
    public override bool TryDeclare(Symbol symbol) => type.TryDeclare(symbol); // TODO: Can we allow it like this?

    /// <inheritdoc />
    protected override bool TryLookupInCurrentScope<TSymbol>(string name, [MaybeNullWhen(false)] out TSymbol symbol) =>
        type.TryLookup(name, out symbol);
}
