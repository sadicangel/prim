using CodeAnalysis.Semantic.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Semantic.Expressions;

internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    VariableSymbol OperatorSymbol,
    BoundExpression Operand)
    : BoundExpression(BoundKind.UnaryExpression, Syntax, ((LambdaSymbol)OperatorSymbol.Type).ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Operand;
    }
}
