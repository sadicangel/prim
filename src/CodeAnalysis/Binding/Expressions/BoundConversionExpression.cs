using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionExpression(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.ConversionExpression, Syntax, FunctionSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Expression;
    }
}
