using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionDeclaration(
    SyntaxNode SyntaxNode,
    ConversionSymbol ConversionSymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.ConversionDeclaration, SyntaxNode, ConversionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return ConversionSymbol;
        yield return Expression;
    }
}
