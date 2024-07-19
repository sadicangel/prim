using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class OptionTypeSymbol : TypeSymbol
{
    public OptionTypeSymbol(SyntaxNode syntax, TypeSymbol underlyingType)
        : base(
            BoundKind.OptionTypeSymbol,
            syntax,
            $"?{underlyingType.Name}",
            PredefinedTypes.Type)
    {
        UnderlyingType = underlyingType;
        AddOperator(
            SyntaxKind.HookHookToken,
            new LambdaTypeSymbol([new Parameter("x", this), new Parameter("y", this)], this));
        AddOperator(
            SyntaxKind.HookHookToken,
            new LambdaTypeSymbol([new Parameter("x", this), new Parameter("y", UnderlyingType)], UnderlyingType));
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", UnderlyingType)], this));
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", PredefinedTypes.Unit)], this));
        AddConversion(
            SyntaxKind.ExplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", this)], UnderlyingType));
    }

    public TypeSymbol UnderlyingType { get; init; }

    public override bool IsNever => UnderlyingType.IsNever;
}
