using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundReturnExpression(
    SyntaxNode Syntax,
    BoundExpression Expression)
    : BoundExpression(BoundKind.ReturnExpression, Syntax, Expression.Type)
{
    public override bool CanJump() => true;

    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}
