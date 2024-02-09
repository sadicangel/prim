namespace CodeAnalysis.Types;

public sealed record class TypeType(PrimType Type) : PrimType(PredefinedSymbolNames.Type)
{
    public bool Equals(TypeType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
