using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class PropertySymbol(
    SyntaxNode Syntax,
    string Name,
    TypeSymbol Type,
    StructTypeSymbol ContainingType,
    Modifiers Modifiers)
    : Symbol(
        SymbolKind.Property,
        Syntax,
        Name,
        Type,
        ContainingType,
        ContainingType.ContainingModule,
        Modifiers)
{
    public override string FullyQualifiedName => $"{ContainingType.FullyQualifiedName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";
    public override string FullName => $"{ContainingType.FullName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public bool Equals(PropertySymbol? other) => other is not null && SymbolKind == other.SymbolKind && FullyQualifiedName == other.FullyQualifiedName;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, FullyQualifiedName);
}
