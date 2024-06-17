using CodeAnalysis.Text;

namespace CodeAnalysis.Types;

public sealed record class UnionType(ReadOnlyList<PrimType> Types)
    : PrimType(string.Join(" | ", Types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)))
{
    public bool Equals(UnionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
