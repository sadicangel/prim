namespace CodeAnalysis.Binding.Types.Metadata;

internal sealed record class Parameter(string Name, PrimType Type)
{
    public override string ToString() => $"{Name}: {Type.Name}";
}
