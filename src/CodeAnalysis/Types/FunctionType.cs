using CodeAnalysis.Operators;

namespace CodeAnalysis.Types;

public sealed record class FunctionType : PrimType
{
    public FunctionType(IReadOnlyList<Parameter> parameters, PrimType returnType)
        : base($"({String.Join(", ", parameters.Select(p => p.ToString()))}) -> {returnType.Name}")
    {
        Parameters = parameters;
        ReturnType = returnType;
        Operators.Add(
            new BinaryOperator(
                OperatorKind.Call,
                this,
                new TypeList([.. Parameters.Select(p => p.Type)]),
                ReturnType));
    }

    public IReadOnlyList<Parameter> Parameters { get; init; }
    public PrimType ReturnType { get; init; }
}

public sealed record class Parameter(string Name, PrimType Type)
{
    public bool Equals(FunctionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => $"{Name}: {Type.Name}";
}
