using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PointerTypeSymbol : TypeSymbol
{
    public PointerTypeSymbol(TypeSymbol elementType)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.PointerType), elementType)
    {
    }

    public PointerTypeSymbol(SyntaxNode syntax, TypeSymbol elementType)
        : base(
            BoundKind.PointerTypeSymbol,
            syntax,
            elementType.IsUnion || elementType.IsLambda ? $"*({elementType.Name})" : $"*{elementType.Name}",
            PredefinedSymbols.Type)
    {
        ElementType = elementType;
    }

    public TypeSymbol ElementType { get; init; }

    public override bool IsNever => ElementType.IsNever;

    internal override bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion)
    {
        conversion = null;
        if (type == this)
        {
            return true;
        }

        conversion = GetConversion(type, this) ?? type.GetConversion(type, this);

        return conversion is not null;
    }
}
