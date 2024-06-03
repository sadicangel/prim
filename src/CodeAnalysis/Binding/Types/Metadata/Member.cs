namespace CodeAnalysis.Binding.Types.Metadata;

internal abstract record class Member(string Name)
{
    public abstract PrimType Type { get; }
}
