using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBreakExpression(
    SyntaxNode Syntax,
    LabelSymbol LabelSymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.BreakExpression, Syntax, Expression.Type)
{
    public override bool CanJump() => true;

    public override IEnumerable<BoundNode> Children()
    {
        yield return LabelSymbol;
        yield return Expression;
    }
}
