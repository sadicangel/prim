using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class StructValue(TypeSymbol TypeSymbol) : PrimValue(PredefinedTypes.Type)
{
    public override PrimType Value => TypeSymbol.Type;
}
