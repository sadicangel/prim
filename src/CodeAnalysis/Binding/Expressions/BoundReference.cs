using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal abstract record class BoundReference(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    Symbol Symbol,
    TypeSymbol Type)
    : BoundExpression(BoundKind, Syntax, Type);
