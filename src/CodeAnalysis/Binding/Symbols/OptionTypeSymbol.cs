using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class OptionTypeSymbol : TypeSymbol
{
    public OptionTypeSymbol(SyntaxNode syntax, TypeSymbol underlyingType, TypeSymbol runtimeType, ModuleSymbol containingModule)
        : base(
            BoundKind.OptionTypeSymbol,
            syntax,
            underlyingType.IsUnion || underlyingType.IsLambda ? $"?({underlyingType.Name})" : $"?{underlyingType.Name}",
            runtimeType,
            containingModule)
    {
        UnderlyingType = underlyingType;
        AddOperator(
            SyntaxKind.HookHookToken,
            containingModule.CreateLambdaType([new Parameter("x", this), new Parameter("y", this)], this));
        AddOperator(
            SyntaxKind.HookHookToken,
            containingModule.CreateLambdaType([new Parameter("x", this), new Parameter("y", UnderlyingType)], UnderlyingType));
    }

    public TypeSymbol UnderlyingType { get; init; }

    public override bool IsNever => UnderlyingType.IsNever;

    internal override bool IsConvertibleFrom(TypeSymbol type, out ConversionSymbol? conversion)
    {
        conversion = null;
        if (type == this || type.IsUnit || type == UnderlyingType)
        {
            return true;
        }

        // TODO: Support underlying type conversions?

        return conversion is not null;
    }
}
