using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundConversionDeclaration(
    SyntaxNode Syntax,
    FunctionSymbol NameSymbol,
    OperatorSymbol OperatorSymbol,
    BoundFunctionBodyExpression Body)
    : BoundMemberDeclaration(BoundKind.ConversionDeclaration, Syntax, NameSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return NameSymbol;
        yield return Body;
    }
}
