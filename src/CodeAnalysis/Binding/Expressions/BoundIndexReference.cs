using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIndexReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    OperatorSymbol OperatorSymbol,
    BoundExpression Index)
    : BoundReference(BoundKind.IndexReference, Syntax, OperatorSymbol, OperatorSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return OperatorSymbol;
        yield return Index;
    }
}
