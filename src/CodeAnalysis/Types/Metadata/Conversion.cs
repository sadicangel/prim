using System.Diagnostics;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Types.Metadata;
public sealed record class Conversion(SyntaxKind ConversionKind, FunctionType Type, PrimType ContainingType)
    : Member(GetConversionName(ConversionKind, Type), ContainingType, IsReadOnly: true, IsStatic: true)
{
    public override FunctionType Type { get; } = Type;
    public bool IsImplicit { get => ConversionKind is SyntaxKind.ImplicitKeyword; }
    public bool IsExplicit { get => ConversionKind is SyntaxKind.ExplicitKeyword; }

    public override string ToString() => $"{Name}: {Type.Name}";

    private static string GetConversionName(SyntaxKind conversionKind, FunctionType type)
    {
        var prefix = conversionKind switch
        {
            SyntaxKind.ImplicitKeyword => "implicit",
            SyntaxKind.ExplicitKeyword => "explicit",
            _ => throw new UnreachableException($"Unexpected conversion '{conversionKind}'")
        };

        var name = $"{prefix}({string.Join(',', type.Parameters.Select(p => p.Type.Name))})->{type.ReturnType}";

        return name;
    }
}
