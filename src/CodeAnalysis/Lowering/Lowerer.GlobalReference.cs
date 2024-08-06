using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundGlobalReference LowerGlobalReference(BoundGlobalReference node, Context context)
    {
        _ = context;
        return node;
    }
}
