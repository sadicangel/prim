using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundLocalReference LowerLocalReference(BoundLocalReference node, LowererContext context)
    {
        _ = context;
        return node;
    }
}
