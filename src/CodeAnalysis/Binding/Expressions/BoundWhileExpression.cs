using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundWhileExpression(
    SyntaxNode Syntax,
    BoundExpression Condition,
    BoundExpression Body)
    : BoundExpression(BoundKind.WhileExpression, Syntax, Body.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Condition;
        yield return Body;
    }
}
