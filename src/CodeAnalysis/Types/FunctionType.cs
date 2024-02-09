namespace CodeAnalysis.Types;

public sealed record class FunctionType(IReadOnlyList<Parameter> Parameters, PrimType ReturnType)
    : PrimType($"({String.Join(", ", Parameters.Select(p => p.ToString()))}) -> {ReturnType.Name}");

public sealed record class Parameter(string Name, PrimType Type)
{
    public bool Equals(FunctionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => $"{Name}: {Type.Name}";
}
