using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class StructTypeSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : TypeSymbol(BoundKind.StructTypeSymbol, Syntax, Name, ContainingModule);
