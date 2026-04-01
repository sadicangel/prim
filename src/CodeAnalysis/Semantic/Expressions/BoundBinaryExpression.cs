using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundBinaryExpression(
    SyntaxNode Syntax,
    BoundExpression Left,
    BoundReference Operator,
    BoundExpression Right)
    : BoundExpression(BoundKind.BinaryExpression, Syntax, ((LambdaTypeSymbol)Operator.Type).ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}
