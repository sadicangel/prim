using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class ErrorTypeSymbol : TypeSymbol
{
    public ErrorTypeSymbol(TypeSymbol elementType)
        : this(SyntaxFactory.SyntheticToken(SyntaxKind.ErrorType), elementType)
    {
    }

    public ErrorTypeSymbol(SyntaxNode syntax, TypeSymbol valueType)
        : base(
            BoundKind.ErrorTypeSymbol,
            syntax,
            valueType.IsUnion || valueType.IsLambda ? $"!({valueType.Name})" : $"!{valueType.Name}",
            PredefinedSymbols.Type)
    {
        ValueType = valueType;
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", ValueType)], this));
        AddConversion(
            SyntaxKind.ImplicitKeyword,
            new LambdaTypeSymbol([new Parameter("x", PredefinedSymbols.Err)], this));
    }

    public TypeSymbol ValueType { get; init; }

    public override bool IsNever => ValueType.IsNever;
}
