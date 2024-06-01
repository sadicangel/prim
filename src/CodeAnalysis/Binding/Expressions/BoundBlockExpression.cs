using CodeAnalysis.Binding.Types;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Binding.Expressions;
internal sealed record class BoundBlockExpression(
    SyntaxNode SyntaxNode,
    BoundList<BoundExpression> Expressions)
    : BoundExpression(BoundKind.BlockExpression, SyntaxNode, GetLastTypeOrNever(Expressions))
{
    public override IEnumerable<BoundNode> Children()
    {
        foreach (var expression in Expressions)
            yield return expression;
    }

    private static PrimType GetLastTypeOrNever(BoundList<BoundExpression> expressions)
    {
        return expressions switch
        {
        [] => PredefinedTypes.Unit,
        [var single] => single.Type,
        [.., { Type.IsNever: true }] => PredefinedTypes.Never,
            _ => expressions[^1].Type
        };
    }
}
