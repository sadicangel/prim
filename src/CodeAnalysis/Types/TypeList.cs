namespace CodeAnalysis.Types;

public sealed record class TypeList(ReadOnlyList<PrimType> Types) : PrimType(String.Join(", ", Types.Select(t => t.Name)))
{
    public bool Equals(TypeList? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
