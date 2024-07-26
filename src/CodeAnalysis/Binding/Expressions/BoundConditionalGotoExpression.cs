using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundConditionalGotoExpression(
    SyntaxNode Syntax,
    LabelSymbol LabelSymbol,
    BoundExpression Condition,
    BoundExpression Expression,
    bool JumpTrue = true)
    : BoundExpression(BoundKind.ConditionalGotoExpression, Syntax, Expression.Type)
{
    public override bool CanJump() => true;

    public override IEnumerable<BoundNode> Children()
    {
        yield return LabelSymbol;
        yield return Condition;
        yield return Expression;
    }
}
