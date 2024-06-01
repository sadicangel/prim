using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal abstract record class BoundExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    PrimType Type)
    : BoundNode(BoundKind, SyntaxNode)
{
}
