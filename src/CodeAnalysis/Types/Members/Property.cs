namespace CodeAnalysis.Types.Members;

public sealed record class Property(
    string Name,
    PrimType Type,
    bool IsMutable
)
    : Member(Name, Type)
{
}
