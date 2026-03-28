using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Declarations;

internal abstract record class BoundDeclaration(BoundKind BoundKind, SyntaxNode Syntax, TypeSymbol Type)
    : BoundExpression(BoundKind, Syntax, Type);
