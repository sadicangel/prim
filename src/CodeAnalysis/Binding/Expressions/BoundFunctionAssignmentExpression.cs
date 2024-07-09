using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundFunctionAssignmentExpression(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundExpression Body)
    : BoundExpression(BoundKind.FunctionBodyExpression, Syntax, FunctionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        yield return Body;
    }
}
