using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(
    BoundKind BoundKind,
    SyntaxNode Syntax,
    BoundExpression Left,
    OperatorSymbol OperatorSymbol,
    BoundExpression Right)
    : BoundExpression(BoundKind, Syntax, OperatorSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return OperatorSymbol;
        yield return Right;
    }
}
