namespace CodeAnalysis.Binding.Types.Metadata;

public sealed record class Property(
    string Name,
    PrimType Type,
    bool IsReadOnly
)
    : Member(Name)
{
}
