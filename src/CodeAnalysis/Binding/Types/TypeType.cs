namespace CodeAnalysis.Binding.Types;

public sealed record class TypeType(PrimType Type) : PrimType(PredefinedTypeNames.Type)
{
    public bool Equals(TypeType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
