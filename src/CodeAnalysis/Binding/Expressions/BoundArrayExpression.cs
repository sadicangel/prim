using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundArrayExpression(
    SyntaxNode Syntax,
    PrimType Type,
    BoundList<BoundExpression> Expressions)
    : BoundExpression(BoundKind.ArrayExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var expression in Expressions)
            yield return expression;
    }
}
