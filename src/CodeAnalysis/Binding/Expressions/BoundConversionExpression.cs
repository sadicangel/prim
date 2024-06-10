using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionExpression(
    SyntaxNode SyntaxNode,
    ConversionSymbol ConversionSymbol,
    BoundExpression Expression)
    : BoundExpression(BoundKind.ConversionExpression, SyntaxNode, ConversionSymbol.Type.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return ConversionSymbol;
        yield return Expression;
    }
}
