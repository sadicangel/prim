using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal sealed record class VariableSymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule,
    bool IsStatic,
    bool IsReadOnly)
    : Symbol(
        BoundKind.VariableSymbol,
        Syntax,
        Name,
        Type,
        ContainingModule,
        IsStatic,
        IsReadOnly)
{
    public override IEnumerable<Symbol> DeclaredSymbols => [];

    public bool Equals(VariableSymbol? other) => base.Equals(other);
    public override int GetHashCode() => base.GetHashCode();

    public static VariableSymbol This(TypeSymbol type, SyntaxNode? syntax = null) => new(
        syntax ?? SyntaxFactory.SyntheticToken(SyntaxKind.ThisKeyword),
        "this",
        type,
        type.ContainingModule,
        IsStatic: false,
        IsReadOnly: true);
}
