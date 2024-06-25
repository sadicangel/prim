using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;

internal abstract record class BoundReference(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    Symbol NameSymbol,
    PrimType Type)
    : BoundExpression(BoundKind, Syntax, Type);
