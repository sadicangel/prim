using CodeAnalysis.Types;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class StructValue(StructType StructType) : PrimValue(PredefinedTypes.Type)
{
    public override StructType Value => StructType;
}
