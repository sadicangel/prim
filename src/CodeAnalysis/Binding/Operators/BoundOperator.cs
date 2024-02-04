using CodeAnalysis.Operators;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Operators;
internal sealed record class BoundOperator(
    SyntaxNode SyntaxNode,
    Operator Operator
)
    : BoundNode(BoundNodeKind.Operator, SyntaxNode)
{
    public PrimType ResultType { get => Operator.ResultType; }

    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
