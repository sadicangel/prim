using System.Diagnostics;
using CodeAnalysis.Binding;
using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundNode LowerNode(BoundNode node, Context context)
    {
        return node switch
        {
            BoundExpression boundExpression => LowerExpression(boundExpression, context),
            _ => throw new UnreachableException($"Unexpected {nameof(BoundKind)} '{node.BoundKind}'"),
        };
    }
}
