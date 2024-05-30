using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundDeclaration(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode)
    : BoundExpression(BoundKind, SyntaxNode);
