using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundBinaryExpression(
    SyntaxNode Syntax,
    BoundExpression Left,
    VariableSymbol Operator,
    BoundExpression Right)
    : BoundExpression(BoundKind.BinaryExpression, Syntax, ((LambdaSymbol)Operator.Type).ReturnType)
{
    public override IEnumerable<ITreeNode> Children()
    {
        yield return Left;
        yield return Operator;
        yield return Right;
    }
}
