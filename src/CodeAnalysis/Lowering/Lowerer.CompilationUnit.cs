using CodeAnalysis.Binding;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundCompilationUnit LowerCompilationUnit(BoundCompilationUnit node, LowererContext context)
    {
        var boundNodes = LowerList(node.BoundNodes, context, LowerNode);
        if (boundNodes is null)
            return node;

        return node with { BoundNodes = new(boundNodes) };
    }
}
