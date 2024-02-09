namespace CodeAnalysis.Types;

public sealed record class ArrayType(PrimType ElementType) : PrimType($"{ElementType.Name}[]")
{
    public bool Equals(ArrayType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
