namespace CodeAnalysis.Binding.Types.Metadata;
internal sealed record class Operator(string Name, FunctionType Type)
    : Member(Name)
{
    public override string ToString() => $"{Name}: {Type.Name}";
}
