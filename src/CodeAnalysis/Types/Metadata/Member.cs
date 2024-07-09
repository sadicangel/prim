namespace CodeAnalysis.Types.Metadata;

public abstract record class Member(
    string Name,
    PrimType ContainingType,
    bool IsReadOnly,
    bool IsStatic)
{
    public abstract PrimType Type { get; }
}
