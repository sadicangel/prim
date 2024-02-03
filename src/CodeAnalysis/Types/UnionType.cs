namespace CodeAnalysis.Types;

public sealed record class UnionType(IReadOnlyList<PrimType> Types)
    : PrimType(String.Join(" | ", Types.Select(t => t.Name).Order(NaturalSortStringComparer.OrdinalIgnoreCase)))
{
    public override bool IsAssignableFrom(PrimType source)
    {
        return this == source || Types.Any(type => type.IsAssignableFrom(source));
    }
}
