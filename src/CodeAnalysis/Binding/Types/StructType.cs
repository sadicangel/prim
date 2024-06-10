namespace CodeAnalysis.Binding.Types;

internal sealed record class StructType(string Name) : PrimType(Name)
{
    public bool Equals(StructType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
