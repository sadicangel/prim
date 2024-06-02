using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(
    BoundKind BoundKind,
    SyntaxNode SyntaxNode,
    OperatorSymbol OperatorSymbol,
    BoundExpression Operand)
    : BoundExpression(BoundKind, SyntaxNode, OperatorSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return OperatorSymbol;
        yield return Operand;
    }
}
