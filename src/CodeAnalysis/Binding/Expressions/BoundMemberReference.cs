using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundMemberReference(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    BoundExpression Expression,
    Symbol Symbol,
    TypeSymbol Type)
    : BoundReference(BoundKind, Syntax, Symbol, Type);
