using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundIfElseExpression(
    SyntaxNode Syntax,
    BoundExpression Condition,
    BoundExpression Then,
    BoundExpression Else,
    PrimType Type)
    : BoundExpression(BoundKind.IfElseExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Condition;
        yield return Then;
        yield return Else;
    }
}
