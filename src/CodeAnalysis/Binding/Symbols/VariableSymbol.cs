using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    bool IsReadOnly)
    : Symbol(
        BoundKind.VariableSymbol,
        Syntax,
        Name,
        Type,
        IsStatic: true,
        IsReadOnly)
{
    public static VariableSymbol This(TypeSymbol type, SyntaxNode? syntax = null) => new(
        syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ThisKeyword),
        "this",
        type,
        IsReadOnly: true);
}
