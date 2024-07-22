using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundMethodGroup(
    SyntaxNode Syntax,
    BoundExpression Expression,
    BoundList<MethodSymbol> MethodSymbols)
    : BoundMemberReference(BoundKind.MethodGroup, Syntax, Expression, MethodSymbols[0], MethodSymbols[0].Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        foreach (var symbol in MethodSymbols)
            yield return symbol;
    }

    public MethodSymbol GetMethod(LambdaTypeSymbol type) => MethodSymbols.Single(m => m.LambdaType == type);
}
