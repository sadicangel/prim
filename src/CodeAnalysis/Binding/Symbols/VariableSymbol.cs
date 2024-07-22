using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    bool IsStatic,
    bool IsReadOnly)
    : Symbol(
        BoundKind.VariableSymbol,
        Syntax,
        Name,
        Type,
        IsStatic,
        IsReadOnly)
{
    public bool Equals(VariableSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    public static VariableSymbol This(TypeSymbol type, SyntaxNode? syntax = null) => new(
        syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ThisKeyword),
        "this",
        type,
        IsStatic: false,
        IsReadOnly: true);
}
