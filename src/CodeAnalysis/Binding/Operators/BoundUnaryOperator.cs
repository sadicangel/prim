using CodeAnalysis.Operators;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Operators;
internal sealed record class BoundUnaryOperator(
    SyntaxNode SyntaxNode,
    UnaryOperator Operator
)
    : BoundOperator(SyntaxNode, Operator.ResultType)
{
    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
