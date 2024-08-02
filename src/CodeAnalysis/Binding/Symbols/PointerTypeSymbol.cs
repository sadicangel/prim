using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class PointerTypeSymbol : TypeSymbol
{
    public PointerTypeSymbol(SyntaxNode syntax, TypeSymbol elementType, TypeSymbol runtimeType, ModuleSymbol containingModule)
        : base(
            BoundKind.PointerTypeSymbol,
            syntax,
            elementType.IsUnion || elementType.IsLambda ? $"*({elementType.Name})" : $"*{elementType.Name}",
            runtimeType,
            containingModule)
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
