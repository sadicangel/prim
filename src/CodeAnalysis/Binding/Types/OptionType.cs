namespace CodeAnalysis.Binding.Types;

public sealed record class OptionType(PrimType UnderlyingType) : PrimType($"{UnderlyingType.Name}?")
{
    public bool Equals(OptionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
