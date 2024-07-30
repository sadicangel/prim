using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundStackInstantiationExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
    BoundExpression Expression)
    : BoundExpression(BoundKind.StackInstantiationExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Type;
        yield return Expression;
    }
}
