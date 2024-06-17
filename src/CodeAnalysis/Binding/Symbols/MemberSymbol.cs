using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Symbols;

internal abstract record class MemberSymbol(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    string Name)
    : Symbol(BoundKind, Syntax, Name);
