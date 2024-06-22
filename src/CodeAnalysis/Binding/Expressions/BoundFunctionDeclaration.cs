using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundFunctionDeclaration(
    SyntaxNode Syntax,
    FunctionSymbol NameSymbol,
    OperatorSymbol OperatorSymbol,
    BoundFunctionBodyExpression Body)
    : BoundDeclaration(BoundKind.FunctionDeclaration, Syntax, NameSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return NameSymbol;
        yield return OperatorSymbol;
        yield return Body;
    }
}
