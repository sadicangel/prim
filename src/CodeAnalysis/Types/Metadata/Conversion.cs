using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types.Metadata;
public sealed record class Conversion(SyntaxKind ConversionKind, FunctionType Type, PrimType ContainingType)
    : Member(SyntaxFacts.GetMethodName(ConversionKind, Type), ContainingType, IsReadOnly: true, IsStatic: true)
{
    public override FunctionType Type { get; } = Type;
    public bool IsImplicit { get => ConversionKind is SyntaxKind.ImplicitKeyword; }
    public bool IsExplicit { get => ConversionKind is SyntaxKind.ExplicitKeyword; }

    public override string ToString() => $"{Name}: {Type.Name}";
}
