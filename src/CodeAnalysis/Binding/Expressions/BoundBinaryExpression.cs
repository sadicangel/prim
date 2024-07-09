using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(
    SyntaxNode Syntax,
    BoundExpression Left,
    FunctionSymbol FunctionSymbol,
    BoundExpression Right)
    : BoundExpression(BoundKind.BinaryExpression, Syntax, FunctionSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return FunctionSymbol;
        yield return Right;
    }
}
