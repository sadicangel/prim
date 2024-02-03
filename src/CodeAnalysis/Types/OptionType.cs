namespace CodeAnalysis.Types;

public sealed record class OptionType(PrimType UnderlyingType)
    : PrimType($"{UnderlyingType.Name}?")
{
    public override bool IsAssignableFrom(PrimType source)
    {
        return this == source || source == PredefinedTypes.Unit || UnderlyingType.IsAssignableFrom(source);
    }
}
