namespace CodeAnalysis.Types;

public sealed record class ArrayType(PrimType ElementType)
    : PrimType($"{ElementType.Name}[]")
{
    public override bool IsAssignableFrom(PrimType source)
    {
        return this == source;
    }
}
