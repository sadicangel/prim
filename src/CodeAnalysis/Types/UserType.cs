namespace CodeAnalysis.Types;

public sealed record class UserType(string Name)
    : PrimType(Name)
{
    public override bool IsAssignableFrom(PrimType source)
    {
        if (this == source) return true;

        // TODO: Check for type custom conversions.

        return false;
    }
}