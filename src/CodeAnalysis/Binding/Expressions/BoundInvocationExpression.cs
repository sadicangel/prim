using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundInvocationExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    OperatorSymbol OperatorSymbol,
    BoundList<BoundExpression> Arguments,
    PrimType Type)
    : BoundExpression(BoundKind.InvocationExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return OperatorSymbol;
        foreach (var argument in Arguments)
            yield return argument;
    }
}
