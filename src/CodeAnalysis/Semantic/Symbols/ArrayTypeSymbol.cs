using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ArrayTypeSymbol(SyntaxNode Syntax, TypeSymbol ElementType, int? Length, ModuleSymbol ContainingModule)
    : TypeSymbol(SymbolKind.Array, Syntax, $"{ElementType.Name}[{Length}]", ContainingModule);
