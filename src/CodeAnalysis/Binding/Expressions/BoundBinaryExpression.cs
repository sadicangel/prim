using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundBinaryExpression(
    SyntaxNode Syntax,
    BoundExpression Left,
    MethodSymbol MethodSymbol,
    BoundExpression Right)
    : BoundExpression(BoundKind.BinaryExpression, Syntax, MethodSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return MethodSymbol;
        yield return Right;
    }
}
