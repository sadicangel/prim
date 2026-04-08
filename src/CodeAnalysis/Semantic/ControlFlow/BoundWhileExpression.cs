using CodeAnalysis.Semantic.Expressions;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.ControlFlow;

internal sealed record class BoundWhileExpression(
    SyntaxNode Syntax,
    BoundExpression Condition,
    BoundExpression Body)
    : BoundExpression(BoundKind.WhileExpression, Syntax, Body.Type)
{
    /// <inheritdoc />
    public override IEnumerable<BoundNode> Children()
    {
        yield return Condition;
        yield return Body;
    }
}
