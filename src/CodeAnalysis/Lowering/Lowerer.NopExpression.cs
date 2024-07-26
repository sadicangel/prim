using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundNopExpression LowerNopExpression(BoundNopExpression node, Context context)
    {
        _ = context;
        return node;
    }
}
