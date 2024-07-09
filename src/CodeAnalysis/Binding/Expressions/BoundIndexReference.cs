using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundIndexReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    FunctionSymbol FunctionSymbol,
    BoundExpression Index)
    : BoundReference(BoundKind.IndexReference, Syntax, FunctionSymbol, FunctionSymbol.ReturnType)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return FunctionSymbol;
        yield return Index;
    }
}
