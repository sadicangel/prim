using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class OptionTypeSymbol : TypeSymbol
{
    public OptionTypeSymbol(TypeSymbol underlyingType)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.OptionType), underlyingType)
    {
    }

    public OptionTypeSymbol(SyntaxNode syntax, TypeSymbol underlyingType)
        : base(
            BoundKind.OptionTypeSymbol,
            syntax,
            underlyingType.IsUnion || underlyingType.IsLambda ? $"?({underlyingType.Name})" : $"?{underlyingType.Name}",
            PredefinedSymbols.Type)
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
            new LambdaTypeSymbol([new Parameter("x", PredefinedSymbols.Unit)], this));
        AddConversion(
            SyntaxKind.ExplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", this)], UnderlyingType));
    }

    public TypeSymbol UnderlyingType { get; init; }

    public override bool IsNever => UnderlyingType.IsNever;
}
