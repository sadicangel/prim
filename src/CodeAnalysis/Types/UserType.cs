namespace CodeAnalysis.Types;

public sealed record class UserType(string Name) : PrimType(Name)
{
    public bool Equals(UserType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}