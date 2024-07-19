using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundDeclaration(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    TypeSymbol Type)
    : BoundExpression(BoundKind, Syntax, Type);
