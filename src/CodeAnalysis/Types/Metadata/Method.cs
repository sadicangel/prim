namespace CodeAnalysis.Types.Metadata;
public sealed record class Method(
    string Name,
    FunctionType Type,
    PrimType ContainingType,
    bool IsStatic)
    : Member(Name, ContainingType, IsReadOnly: true, IsStatic)
{
    public override FunctionType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
