using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBlockExpression(
    SyntaxNode Syntax,
    PrimType Type,
    BoundList<BoundExpression> Expressions)
    : BoundExpression(BoundKind.BlockExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var expression in Expressions)
            yield return expression;
    }
}
