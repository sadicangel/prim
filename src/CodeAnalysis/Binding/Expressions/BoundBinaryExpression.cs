using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(
    SyntaxNode Syntax,
    BoundExpression Left,
    OperatorSymbol OperatorSymbol,
    BoundExpression Right)
    : BoundExpression(BoundKind.BinaryExpression, Syntax, OperatorSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return OperatorSymbol;
        yield return Right;
    }
}
