using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class PointerTypeSymbol(SyntaxNode Syntax, TypeSymbol ElementType, ModuleSymbol ContainingModule)
    : TypeSymbol(BoundKind.PointerTypeSymbol, Syntax, $"{ElementType.Name}*", ContainingModule);
