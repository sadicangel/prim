using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    BoundReference Operator,
    BoundExpression Operand)
    : BoundExpression(BoundKind.UnaryExpression, Syntax, ((LambdaTypeSymbol)Operator.Type).ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Operator;
        yield return Operand;
    }
}
