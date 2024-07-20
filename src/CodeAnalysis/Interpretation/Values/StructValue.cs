using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class StructValue(TypeSymbol TypeSymbol) : PrimValue(PredefinedSymbols.Type)
{
    public override TypeSymbol Value => TypeSymbol;
}
