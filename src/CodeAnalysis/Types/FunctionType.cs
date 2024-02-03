using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types;

public sealed record class FunctionType(IReadOnlyList<ParameterSyntax> Parameters, PrimType ReturnType)
    : PrimType($"({String.Join(", ", Parameters.Select(p => $"{p.Identifier.Text}{p.Colon.Text} {p.Type.Text}"))}) -> {ReturnType.Name}")
{
    public override bool IsAssignableFrom(PrimType source)
    {
        return this == source;
    }
}
