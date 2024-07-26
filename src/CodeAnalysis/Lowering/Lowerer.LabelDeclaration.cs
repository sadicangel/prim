using CodeAnalysis.Binding.Expressions;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundLabelDeclaration LowerLabelDeclaration(BoundLabelDeclaration node, LowererContext context)
    {
        _ = context;
        return node;
    }
}
