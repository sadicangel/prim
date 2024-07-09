using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundFunctionDeclaration(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Body)
    : BoundDeclaration(BoundKind.FunctionDeclaration, Syntax, FunctionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Body;
    }
}
