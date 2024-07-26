using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundLocalReference LowerLocalReference(BoundLocalReference node, Context context)
    {
        _ = context;
        return node;
    }
}
