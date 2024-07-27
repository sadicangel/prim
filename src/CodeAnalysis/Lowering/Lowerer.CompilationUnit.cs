using CodeAnalysis.Binding;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundCompilationUnit LowerCompilationUnit(BoundCompilationUnit node, Context context)
    {
        var boundNodes = LowerList(node.BoundNodes, context, LowerNode);
        if (boundNodes.IsDefault)
            return node;

        return node with { BoundNodes = new(boundNodes) };
    }
}
