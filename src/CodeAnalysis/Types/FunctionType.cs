namespace CodeAnalysis.Types;

public sealed record class FunctionType(IReadOnlyList<Parameter> Parameters, PrimType ReturnType)
    : PrimType($"({String.Join(", ", Parameters.Select(p => p.ToString()))}) -> {ReturnType.Name}")
{
    public override bool IsAssignableFrom(PrimType source)
    {
        return this == source;
    }
}

public sealed record class Parameter(string Name, PrimType Type)
{
    public override string ToString() => $"{Name}: {Type.Name}";
}
