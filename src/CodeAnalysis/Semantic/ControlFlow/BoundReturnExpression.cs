using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.ControlFlow;

internal sealed record class BoundReturnExpression(
    SyntaxNode Syntax,
    BoundExpression Expression)
    : BoundExpression(BoundKind.ReturnExpression, Syntax, Expression.Type)
{
    public override bool CanExitScope => true;

    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
    }
}
