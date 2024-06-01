namespace CodeAnalysis.Binding.Types.Metadata;

public sealed record class Parameter(string Name, PrimType Type)
{
    public override string ToString() => $"{Name}: {Type.Name}";
}
