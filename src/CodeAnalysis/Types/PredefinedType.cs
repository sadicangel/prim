
namespace CodeAnalysis.Types;
public sealed record class PredefinedType(string Name) : PrimType(Name)
{
    public bool Equals(PredefinedType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
