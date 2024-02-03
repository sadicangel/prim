namespace CodeAnalysis.Types;

public sealed record class TypeType(PrimType Type)
    : PrimType(PredefinedTypeNames.Type)
{
    public override bool IsAssignableFrom(PrimType source) => this == source;
}
