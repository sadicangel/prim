using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ErrorTypeSymbol : TypeSymbol
{
    public ErrorTypeSymbol(SyntaxNode syntax, TypeSymbol valueType, ModuleSymbol containingModule)
        : base(
            BoundKind.ErrorTypeSymbol,
            syntax,
            valueType.IsUnion || valueType.IsLambda ? $"!({valueType.Name})" : $"!{valueType.Name}",
            Predefined.Type,
            containingModule)
    {
        ValueType = valueType;
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", ValueType)], this, containingModule));
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", Predefined.Err)], this, containingModule));
    }

    public TypeSymbol ValueType { get; init; }

    public override bool IsNever => ValueType.IsNever;

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
