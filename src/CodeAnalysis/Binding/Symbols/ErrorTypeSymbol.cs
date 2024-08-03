using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ErrorTypeSymbol : TypeSymbol
{
    public ErrorTypeSymbol(SyntaxNode syntax, TypeSymbol errType, TypeSymbol valueType, TypeSymbol runtimeType, ModuleSymbol containingModule)
        : base(
            BoundKind.ErrorTypeSymbol,
            syntax,
            valueType.IsUnion || valueType.IsLambda ? $"!({valueType.Name})" : $"!{valueType.Name}",
            runtimeType,
            containingModule)
    {
        ValueType = valueType;
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            containingModule.CreateLambdaType([new Parameter("x", ValueType)], this));
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            containingModule.CreateLambdaType([new Parameter("x", errType)], this));
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
