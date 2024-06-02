namespace CodeAnalysis.Binding.Types;

internal sealed record class NamedType(string Name) : PrimType(Name)
{
    public bool Equals(NamedType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
