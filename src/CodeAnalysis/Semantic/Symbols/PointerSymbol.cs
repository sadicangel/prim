using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class PointerSymbol(SyntaxNode Syntax, TypeSymbol ElementType, ModuleSymbol ContainingModule)
    : TypeSymbol(SymbolKind.Pointer, Syntax, $"{ElementType.Name}*", ContainingModule);
