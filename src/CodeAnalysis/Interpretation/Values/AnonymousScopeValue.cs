using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class AnonymousScopeValue(
    AnonymousScopeSymbol AnonymousScopeSymbol,
    ModuleValue ContainingModule,
    ScopeValue ContainingScope)
    : ScopeValue(
        AnonymousScopeSymbol,
        ContainingModule,
        ContainingScope)
{
    public override ModuleValue Module => ContainingModule;
}

