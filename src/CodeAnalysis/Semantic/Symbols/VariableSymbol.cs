using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class VariableSymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    ModuleSymbol ContainingModule,
    Modifiers Modifiers)
    : Symbol(
        SymbolKind.Variable,
        Syntax,
        Name,
        Type,
        ContainingModule,
        ContainingModule,
        Modifiers)
{
    public bool Equals(VariableSymbol? other) => other is not null && SymbolKind == other.SymbolKind && FullyQualifiedName == other.FullyQualifiedName;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, FullyQualifiedName);
}
