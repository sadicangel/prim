using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode)
    : BoundNode(BoundKind, SyntaxNode)
{
}
