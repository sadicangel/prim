using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class IndexerSymbol(
    SyntaxNode Syntax,
    TypeSymbol IndexType,
    TypeSymbol ElementType,
    TypeSymbol ContainingType,
    Modifiers Modifiers)
    : Symbol(
        SymbolKind.Indexer,
        Syntax,
        $"[{IndexType.Name}]",
        ElementType,
        ContainingType,
        ContainingType.ContainingModule,
        Modifiers)
{
    public override string FullyQualifiedName => $"{ContainingType.FullyQualifiedName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public override string FullName => $"{ContainingType.FullName}{SyntaxFacts.GetText(SyntaxKind.ColonColonToken)}{Name}";

    public bool Equals(IndexerSymbol? other) => other is not null && SymbolKind == other.SymbolKind && FullyQualifiedName == other.FullyQualifiedName;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, FullyQualifiedName);
}
