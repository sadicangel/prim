using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    VariableSymbol Operator,
    BoundExpression Operand)
    : BoundExpression(BoundKind.UnaryExpression, Syntax, ((LambdaSymbol)Operator.Type).ReturnType)
{
    public override IEnumerable<ITreeNode> Children()
    {
        yield return Operator;
        yield return Operand;
    }
}
