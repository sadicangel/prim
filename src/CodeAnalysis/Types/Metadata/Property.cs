namespace CodeAnalysis.Types.Metadata;

public sealed record class Property(string Name, PrimType Type, PrimType ContainingType, bool IsReadOnly)
    : Member(Name, ContainingType)
{
    public override PrimType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
