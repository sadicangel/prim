using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundContinueExpression(
    SyntaxNode Syntax,
    LabelSymbol LabelSymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.ContinueExpression, Syntax, Expression.Type)
{
    public override bool CanJump() => true;

    public override IEnumerable<BoundNode> Children()
    {
        yield return LabelSymbol;
        yield return Expression;
    }
}
