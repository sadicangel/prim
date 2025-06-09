using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundBinaryExpression(
    SyntaxNode Syntax,
    BoundExpression Left,
    VariableSymbol OperatorSymbol,
    BoundExpression Right)
    : BoundExpression(BoundKind.BinaryExpression, Syntax, ((LambdaSymbol)OperatorSymbol.Type).ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Right;
    }
}
