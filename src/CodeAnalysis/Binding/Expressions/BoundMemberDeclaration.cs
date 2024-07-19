using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundMemberDeclaration(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundDeclaration(BoundKind, Syntax, Type);
