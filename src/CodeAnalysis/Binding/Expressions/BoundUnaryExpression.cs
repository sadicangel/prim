using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    OperatorSymbol OperatorSymbol,
    BoundExpression Operand)
    : BoundExpression(BoundKind.UnaryExpression, Syntax, OperatorSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return OperatorSymbol;
        yield return Operand;
    }
}
