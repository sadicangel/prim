using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class LambdaTypeSymbol(
    SyntaxNode Syntax,
    ImmutableArray<ParameterSymbol> Parameters,
    TypeSymbol ReturnType,
    ModuleSymbol ContainingModule)
    : TypeSymbol(
        BoundKind.LambdaTypeSymbol,
        Syntax,
        $"({string.Join(", ", Parameters.Select(p => p.ToString()))}) -> {ReturnType.Name}",
        ContainingModule);
