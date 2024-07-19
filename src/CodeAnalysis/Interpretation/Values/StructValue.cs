using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class StructValue(TypeSymbol TypeSymbol) : PrimValue(PredefinedTypes.Type)
{
    public override TypeSymbol Value => TypeSymbol;
}
