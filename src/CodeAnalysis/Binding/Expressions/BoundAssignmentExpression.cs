using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundAssignmentExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Expression
)
    : BoundExpression(BoundNodeKind.AssignmentExpression, SyntaxNode, Expression.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}
