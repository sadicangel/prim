using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundLabelDeclaration LowerLabelDeclaration(BoundLabelDeclaration node, Context context)
    {
        _ = context;
        return node;
    }
}
