using CodeAnalysis.Syntax;
using CodeAnalysis.Types.Metadata;

namespace CodeAnalysis.Types;

public sealed record class FunctionType : PrimType
{
    public FunctionType(ReadOnlyList<Parameter> parameters, PrimType returnType)
        : base($"({string.Join(", ", parameters.Select(p => p.ToString()))}) -> {returnType.Name}")
    {
        Parameters = parameters;
        ReturnType = returnType;
        AddOperator(SyntaxKind.ParenthesisOpenParenthesisCloseToken, this);
    }

    public ReadOnlyList<Parameter> Parameters { get; init; }
    public PrimType ReturnType { get; init; }
    public bool Equals(FunctionType? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();
}
