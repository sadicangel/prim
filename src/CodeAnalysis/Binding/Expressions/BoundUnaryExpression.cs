using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    MethodSymbol MethodSymbol,
    BoundExpression Operand)
    : BoundExpression(BoundKind.UnaryExpression, Syntax, MethodSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return MethodSymbol;
        yield return Operand;
    }
}
