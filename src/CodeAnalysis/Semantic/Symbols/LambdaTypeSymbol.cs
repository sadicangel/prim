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
        ContainingModule);
