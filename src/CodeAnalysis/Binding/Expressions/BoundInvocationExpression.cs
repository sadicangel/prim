using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundInvocationExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    BoundList<BoundExpression> Arguments,
    PrimType Type)
    : BoundExpression(BoundKind.InvocationExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        foreach (var argument in Arguments)
            yield return argument;
    }
}
