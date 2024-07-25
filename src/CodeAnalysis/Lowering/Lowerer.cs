using CodeAnalysis.Binding;

namespace CodeAnalysis.Lowering;
internal static partial class Lowerer
{
    public static BoundTree Lower(BoundTree boundTree)
    {
        var compilationUnit = LowerCompilationUnit(boundTree.CompilationUnit);
        if (ReferenceEquals(compilationUnit, boundTree.CompilationUnit))
            return boundTree;
        return boundTree with { CompilationUnit = compilationUnit };
    }


    private static List<T>? LowerList<T>(BoundList<T> nodes, Func<T, T> lower) where T : BoundNode
    {
        List<T>? boundNodes = null;
        for (var i = 0; i < nodes.Count; ++i)
        {
            var oldNode = nodes[i];
            var newNode = lower(oldNode);
            if (!ReferenceEquals(newNode, oldNode) && boundNodes is null)
            {
                boundNodes = new List<T>(nodes.Count);
                for (var j = 0; j < i; ++j)
                    boundNodes.Add(nodes[j]);
            }
            boundNodes?.Add(newNode);
        }

        return boundNodes;
    }
}
