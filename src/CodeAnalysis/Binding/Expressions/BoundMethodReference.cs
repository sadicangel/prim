using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundMethodReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    MethodSymbol MethodSymbol)
    : BoundMemberReference(BoundKind.MethodReference, Syntax, Expression, MethodSymbol, MethodSymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Symbol;
        yield return Type;
    }
}
