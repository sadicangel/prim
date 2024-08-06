using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStackInstantiation(
    SyntaxNode Syntax,
    BoundExpression Expression,
    TypeSymbol Type)
    : BoundExpression(BoundKind.StackInstantiation, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Type;
    }
}
