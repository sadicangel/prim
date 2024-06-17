using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    OperatorSymbol OperatorSymbol,
    BoundExpression Operand)
    : BoundExpression(BoundKind, Syntax, OperatorSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return OperatorSymbol;
        yield return Operand;
    }
}
