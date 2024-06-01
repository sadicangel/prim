using CodeAnalysis.Binding.Types.Metadata;

namespace CodeAnalysis.Binding.Types;

public sealed record class FunctionType : PrimType
{
    public FunctionType(ReadOnlyList<Parameter> parameters, PrimType returnType)
        : base($"({string.Join(", ", parameters.Select(p => p.ToString()))}) -> {returnType.Name}")
    {
        Parameters = parameters;
        ReturnType = returnType;
    }

    public IReadOnlyList<Parameter> Parameters { get; init; }
    public PrimType ReturnType { get; init; }
    public bool Equals(FunctionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
