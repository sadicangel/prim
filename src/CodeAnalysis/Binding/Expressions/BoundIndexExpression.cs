using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIndexExpression(
    SyntaxNode Syntax,
    BoundExpression Expression,
    OperatorSymbol OperatorSymbol,
    BoundExpression Index)
    : BoundExpression(BoundKind.IndexExpression, Syntax, OperatorSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return OperatorSymbol;
        yield return Index;
    }
}
