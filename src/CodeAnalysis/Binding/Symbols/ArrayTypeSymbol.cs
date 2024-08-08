using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ArrayTypeSymbol : TypeSymbol
{
    public ArrayTypeSymbol(SyntaxNode syntax, TypeSymbol elementType, int length, ModuleSymbol containingModule)
        : base(
            BoundKind.ArrayTypeSymbol,
            syntax,
            $"[{elementType.Name}: {length}]",
            containingModule)
    {
        ElementType = elementType;
        Length = length;
        IndexerType = containingModule.Usz;
        AddOperator(
            SyntaxKind.BracketOpenBracketCloseToken,
            new LambdaTypeSymbol([new("index", containingModule.Usz)], ElementType, containingModule));
    }

    public TypeSymbol ElementType { get; init; }
    public int Length { get; init; }

    public TypeSymbol IndexerType { get; init; }

    public override bool IsNever => ElementType.IsNever;

    internal override bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion)
    {
        conversion = null;

        if (type is not ArrayTypeSymbol arrayType)
        {
            return false;
        }

        if (arrayType == this)
        {
            return true;
        }

        if (!ElementType.IsConvertibleFrom(arrayType.ElementType, out conversion))
        {
            return false;
        }

        if (conversion is null)
        {
            return true;
        }

        var conversionType = new LambdaTypeSymbol([new Parameter("x", type)], this, ContainingModule);

        _ = AddConversion(conversion.ConversionKind, conversionType);

        conversion = GetConversion(conversionType);

        return true;
    }
}
