using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Operators;
internal abstract record class BoundOperator(
    SyntaxNode SyntaxNode,
    PrimType ResultType
)
    : BoundNode(BoundNodeKind.Operator, SyntaxNode);
