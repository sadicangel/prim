using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.ControlFlow;

internal sealed record class BoundBreakExpression(
    SyntaxNode Syntax,
    BoundExpression Expression)
    : BoundExpression(BoundKind.BreakExpression, Syntax, Expression.Type)
{
    public override bool CanExitScope => true;

    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}
