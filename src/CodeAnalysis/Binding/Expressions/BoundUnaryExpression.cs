using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundUnaryExpression(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Operand)
    : BoundExpression(BoundKind.UnaryExpression, Syntax, FunctionSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Operand;
    }
}
