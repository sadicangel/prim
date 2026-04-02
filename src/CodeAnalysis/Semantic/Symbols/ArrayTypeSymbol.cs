using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ArrayTypeSymbol : TypeSymbol
{
    public TypeSymbol ElementType { get; init; }
    public int? Length { get; init; }

    public ArrayTypeSymbol(SyntaxNode Syntax, TypeSymbol ElementType, int? Length, ModuleSymbol ContainingModule)
        : base(SymbolKind.Array, Syntax, $"{ElementType.Name}[{Length}]", ContainingModule)
    {
        this.ElementType = ElementType;
        this.Length = Length;
        TryDeclare(CreateIndexer(this, ElementType));
    }

    public bool Equals(ArrayTypeSymbol? other) => other is not null && SymbolKind == other.SymbolKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, Name);

    private static IndexerSymbol CreateIndexer(ArrayTypeSymbol @this, TypeSymbol elementType)
    {
        var @operator = SyntaxToken.CreateSynthetic(SyntaxKind.BracketOpenBracketCloseToken);

        // TODO: Use usz for the index type when it is supported.
        return new IndexerSymbol(@operator, @this.ContainingModule.I32, elementType, @this, Modifiers.None);
    }
};
