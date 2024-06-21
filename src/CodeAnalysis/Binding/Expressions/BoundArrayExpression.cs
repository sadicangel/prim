using CodeAnalysis.Syntax;
using CodeAnalysis.Types;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundArrayExpression(
    SyntaxNode Syntax,
    PrimType Type,
    BoundList<BoundExpression> Elements)
    : BoundExpression(BoundKind.ArrayExpression, Syntax, Type)
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var element in Elements)
            yield return element;
    }
}
