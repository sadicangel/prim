namespace CodeAnalysis.Types.Metadata;

public abstract record class Member(string Name)
{
    public abstract PrimType Type { get; }
}
