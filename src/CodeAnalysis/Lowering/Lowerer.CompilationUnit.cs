using CodeAnalysis.Binding;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static BoundCompilationUnit LowerCompilationUnit(BoundCompilationUnit node)
    {
        var boundNodes = LowerList(node.BoundNodes, LowerNode);
        if (boundNodes is null)
            return node;

        return node with { BoundNodes = new(boundNodes) };
    }
}
