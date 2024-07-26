using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundLiteralExpression LowerLiteralExpression(BoundLiteralExpression node, LowererContext context)
    {
        _ = context;
        return node;
    }
}
