namespace CodeAnalysis.Binding.Types.Metadata;

internal sealed record class Property(
    string Name,
    PrimType Type,
    bool IsReadOnly
)
    : Member(Name)
{
    public override PrimType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
