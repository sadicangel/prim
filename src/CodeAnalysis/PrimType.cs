using CodeAnalysis.Syntax;
namespace CodeAnalysis;

public abstract record class PrimType(string Name);
public sealed record class PredefinedType(string Name) : PrimType(Name);
public sealed record class ReferencedType(string Name) : PrimType(Name);
public sealed record class OptionType(PrimType UnderlyingType) : PrimType($"{UnderlyingType.Name}?");
public sealed record class UnionType(IReadOnlyList<PrimType> Types) : PrimType(String.Join(" | ", Types.Select(t => t.Name)));
public sealed record class ArrayType(PrimType ElementType) : PrimType($"{ElementType.Name}[]");
public sealed record class FunctionType(IReadOnlyList<Parameter> Parameters, PrimType ReturnType)
    : PrimType($"({String.Join(", ", Parameters.Select(p => $"{p.Identifier.Text}{p.Colon.Text} {p.Type.Text}"))}) -> {ReturnType.Name}");

public sealed record class Parameter(
    SyntaxTree SyntaxTree,
    Token Identifier,
    Token Colon,
    TypeSyntax Type
)
    : SyntaxNode(SyntaxNodeKind.Parameter, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Identifier;
        yield return Colon;
        yield return Type;
    }
}