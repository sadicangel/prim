namespace CodeAnalysis.Binding.Types;

internal sealed record class ArrayType(PrimType ElementType, int Length) : PrimType($"[{ElementType.Name}: {Length}]")
{
    public bool Equals(ArrayType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
