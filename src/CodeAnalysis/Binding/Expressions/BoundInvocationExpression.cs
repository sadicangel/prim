using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundInvocationExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    MethodSymbol MethodSymbol,
    BoundList<BoundExpression> Arguments)
    : BoundExpression(BoundKind.InvocationExpression, Syntax, MethodSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return MethodSymbol;
        foreach (var argument in Arguments)
            yield return argument;
    }
}
