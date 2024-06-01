namespace CodeAnalysis.Binding.Types;

public sealed record class TypeList(ReadOnlyList<PrimType> Types) : PrimType(string.Join(", ", Types.Select(t => t.Name)))
{
    public bool Equals(TypeList? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
