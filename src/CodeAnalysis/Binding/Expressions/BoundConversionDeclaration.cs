using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionDeclaration(
    SyntaxNode Syntax,
    ConversionSymbol ConversionSymbol,
    BoundExpression Expression)
    : BoundMemberDeclaration(BoundKind.ConversionDeclaration, Syntax, ConversionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return ConversionSymbol;
        yield return Expression;
    }
}
