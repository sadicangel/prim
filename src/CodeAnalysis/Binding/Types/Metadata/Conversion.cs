using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Types.Metadata;
internal sealed record class Conversion(SyntaxKind ConversionKind, FunctionType Type)
    : Member($"{(ConversionKind is SyntaxKind.ImplicitKeyword ? "implicit" : "explicit")}<{Type.Name}>")
{
    public override FunctionType Type { get; } = Type;
    public override string ToString() => $"{Name}: {Type.Name}";
}
