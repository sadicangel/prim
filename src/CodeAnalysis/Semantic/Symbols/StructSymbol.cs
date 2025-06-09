using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Symbols;

internal sealed record class StructSymbol(SyntaxNode Syntax, string Name, ModuleSymbol ContainingModule)
    : TypeSymbol(SymbolKind.Struct, Syntax, Name, ContainingModule);
