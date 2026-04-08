using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ConversionSymbol(
    ConversionKind ConversionKind,
    SyntaxNode Syntax,
    LambdaTypeSymbol LambdaType,
    StructTypeSymbol ContainingType)
    : Symbol(
        SymbolKind.Conversion,
        Syntax,
        GetConversionName(LambdaType.Parameters[0], LambdaType.ReturnType),
        LambdaType,
        ContainingType,
        ContainingType.ContainingModule,
        Modifiers.ReadOnly | Modifiers.Static)
{
    public static string GetConversionName(TypeSymbol source, TypeSymbol target) =>
        $"op_{target.Name}({source.Name})";
}
