using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundInvocationExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    OperatorSymbol OperatorSymbol,
    BoundList<BoundExpression> Arguments)
    : BoundExpression(BoundKind.InvocationExpression, Syntax, OperatorSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return OperatorSymbol;
        foreach (var argument in Arguments)
            yield return argument;
    }
}
