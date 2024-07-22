using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;

internal sealed record class BoundPropertyReference(
    SyntaxNode Syntax,
    BoundExpression Expression,
    PropertySymbol PropertySymbol)
    : BoundMemberReference(BoundKind.PropertyReference, Syntax, Expression, PropertySymbol, PropertySymbol.Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        yield return Expression;
        yield return Symbol;
        yield return Type;
    }
}
