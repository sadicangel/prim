using CodeAnalysis.Binding.Symbols;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundArrayInitExpression(
    SyntaxNode Syntax,
    TypeSymbol Type,
    BoundList<BoundExpression> Elements)
    : BoundExpression(BoundKind.ArrayInitExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var element in Elements)
            yield return element;
    }
}
