using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundArrayInitExpression(
    SyntaxNode Syntax,
    PrimType Type,
    BoundList<BoundExpression> Elements)
    : BoundExpression(BoundKind.ArrayInitExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var element in Elements)
            yield return element;
    }
}
