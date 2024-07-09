using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(
    SyntaxNode Syntax,
    string Name,
    PrimType Type,
    bool IsReadOnly)
    : Symbol(BoundKind.VariableSymbol, Syntax, Name, Type, IsReadOnly, IsStatic: true)
{
    public static VariableSymbol This(PrimType type, SyntaxNode? syntax = null) => new(
        syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ThisKeyword),
        "this",
        type,
        IsReadOnly: true);
}
