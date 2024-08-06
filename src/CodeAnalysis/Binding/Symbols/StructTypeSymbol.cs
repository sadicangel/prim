using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class StructTypeSymbol(
    SyntaxNode Syntax,
    string Name,
    ModuleSymbol ContainingModule)
    : TypeSymbol(
        BoundKind.StructTypeSymbol,
        Syntax,
        Name,
        ContainingModule)
{
    public override bool IsNever => Name == "never";

    internal override bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion)
    {
        conversion = null;
        if (IsAny || type == this)
        {
            return true;
        }

        conversion = GetConversion(type, this) ?? type.GetConversion(type, this);

        return conversion is not null;
    }
}
