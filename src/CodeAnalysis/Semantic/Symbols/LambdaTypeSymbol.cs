using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class LambdaTypeSymbol(
    SyntaxNode Syntax,
    ImmutableArray<TypeSymbol> Parameters,
    TypeSymbol ReturnType,
    ModuleSymbol ContainingModule)
    : TypeSymbol(
        SymbolKind.Lambda,
        Syntax,
        $"({string.Join(", ", Parameters.Select(p => p.Name))}) -> {ReturnType.Name}",
        ContainingModule)
{
    public bool Equals(LambdaTypeSymbol? other) => other is not null && SymbolKind == other.SymbolKind && Name == other.Name;
    public override int GetHashCode() => HashCode.Combine(SymbolKind, Name);
}
