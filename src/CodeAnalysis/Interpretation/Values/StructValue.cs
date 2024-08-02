using CodeAnalysis.Binding.Symbols;

namespace CodeAnalysis.Interpretation.Values;

internal sealed record class StructValue(StructTypeSymbol StructType, TypeSymbol Type) : PrimValue(Type)
{
    public string Name { get => StructType.Name; }
    public override StructTypeSymbol Value => StructType;

    public bool Equals(StructValue? other) => Name == other?.Name;
    public override int GetHashCode() => Name.GetHashCode();
}
