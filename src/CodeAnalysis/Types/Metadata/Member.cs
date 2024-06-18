namespace CodeAnalysis.Types.Metadata;

public abstract record class Member(string Name, PrimType ContainingType)
{
    public abstract PrimType Type { get; }
}
