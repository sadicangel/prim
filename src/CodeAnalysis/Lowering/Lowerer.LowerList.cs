using CodeAnalysis.Binding;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static List<T>? LowerList<T>(BoundList<T> nodes, LowererContext context, Func<T, LowererContext, T> lower) where T : BoundNode
    {
        List<T>? boundNodes = null;
        for (var i = 0; i < nodes.Count; ++i)
        {
            var oldNode = nodes[i];
            var newNode = lower(oldNode, context);
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
