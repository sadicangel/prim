namespace CodeAnalysis.Types.Metadata;

public sealed record class Property(string Name, PrimType Type, PrimType ContainingType, bool IsReadOnly, bool IsStatic)
    : Member(Name, ContainingType, IsReadOnly, IsStatic)
{
    public override PrimType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
