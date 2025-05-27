using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class ArrayTypeSymbol(SyntaxNode Syntax, TypeSymbol ElementType, int Length, ModuleSymbol ContainingModule)
    : TypeSymbol(BoundKind.ArrayTypeSymbol, Syntax, $"{ElementType.Name}[{Length}]", ContainingModule);
