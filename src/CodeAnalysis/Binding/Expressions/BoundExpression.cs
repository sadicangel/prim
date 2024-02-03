using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;

internal abstract record class BoundExpression(
    BoundNodeKind NodeKind,
    SyntaxNode SyntaxNode,
    PrimType Type
)
    : BoundNode(NodeKind, SyntaxNode);