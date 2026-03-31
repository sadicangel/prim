using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class PointerTypeSymbol(SyntaxNode Syntax, TypeSymbol ElementType, ModuleSymbol ContainingModule)
    : TypeSymbol(SymbolKind.Pointer, Syntax, $"{ElementType.Name}*", ContainingModule);
