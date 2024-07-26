using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundLiteralExpression LowerLiteralExpression(BoundLiteralExpression node, Context context)
    {
        _ = context;
        return node;
    }
}
