using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundFunctionBodyExpression(
    SyntaxNode Syntax,
    FunctionSymbol FunctionSymbol,
    BoundList<VariableSymbol> ParameterSymbols,
    BoundExpression Expression)
    : BoundExpression(BoundKind.FunctionBodyExpression, Syntax, FunctionSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return FunctionSymbol;
        foreach (var parameterSymbol in ParameterSymbols)
            yield return parameterSymbol;
        yield return Expression;
    }
}
