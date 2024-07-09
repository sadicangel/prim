using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundInvocationExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    FunctionSymbol FunctionSymbol,
    BoundList<BoundExpression> Arguments)
    : BoundExpression(BoundKind.InvocationExpression, Syntax, FunctionSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return FunctionSymbol;
        foreach (var argument in Arguments)
            yield return argument;
    }
}
