using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionDeclaration(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Body)
    : BoundMemberDeclaration(BoundKind.ConversionDeclaration, Syntax, FunctionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Body;
    }
}
