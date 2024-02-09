namespace CodeAnalysis.Types;

public sealed record class UnionType(IReadOnlyList<PrimType> Types)
    : PrimType(String.Join(" | ", Types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)))
{
    public bool Equals(UnionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
