using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class StructValue(StructSymbol StructSymbol) : PrimValue(PredefinedTypes.Type)
{
    public override StructType Value => StructSymbol.StructType;
}
