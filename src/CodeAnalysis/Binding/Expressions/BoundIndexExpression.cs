using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIndexExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    BoundExpression Index,
    PrimType Type)
    : BoundExpression(BoundKind.IndexExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Index;
    }
}
