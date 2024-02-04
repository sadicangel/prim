using CodeAnalysis.Operators;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Operators;
internal sealed record class BoundBinaryOperator(
    SyntaxNode SyntaxNode,
    BinaryOperator Operator
)
    : BoundOperator(SyntaxNode, Operator.ResultType)
{
    public override IEnumerable<BoundNode> Children() => Enumerable.Empty<BoundNode>();
}
