using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(
    SyntaxNode Syntax,
    string Name,
    PrimType Type,
    Symbol? ContainingSymbol,
    bool IsReadOnly)
    : Symbol(
        BoundKind.VariableSymbol,
        Syntax,
        Name,
        Type,
        ContainingSymbol,
        IsReadOnly,
        IsStatic: true)
{
    public static VariableSymbol This(PrimType type, Symbol containingSymbol, SyntaxNode? syntax = null) => new(
        syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ThisKeyword),
        "this",
        type,
        containingSymbol,
        IsReadOnly: true);
}
