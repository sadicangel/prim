using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIfElseExpression(
    SyntaxNode SyntaxNode,
    BoundExpression Condition,
    BoundExpression Then,
    BoundExpression Else
)
    : BoundExpression(BoundNodeKind.IfElseExpression, SyntaxNode, Then.Type == Else.Type ? Then.Type : new UnionType([Then.Type, Else.Type]))
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Condition;
        yield return Then;
        yield return Else;
    }
}
