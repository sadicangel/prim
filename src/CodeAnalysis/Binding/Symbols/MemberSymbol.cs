using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal abstract record class MemberSymbol(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    string Name)
    : Symbol(BoundKind, SyntaxNode, Name);
