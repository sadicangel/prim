using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class AnonymousScopeValue(
    AnonymousScopeSymbol AnonymousScopeSymbol,
    ScopeValue ContainingScope)
    : ScopeValue(
        AnonymousScopeSymbol,
        ContainingScope.Module,
        ContainingScope)
{
    public override ModuleValue Module => ContainingModule;

    public override object Value => AnonymousScopeSymbol.Name;
}

