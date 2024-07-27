using System.Collections.Immutable;
using CodeAnalysis.Binding;

namespace CodeAnalysis.Lowering;
partial class Lowerer
{
    private static ImmutableArray<T> LowerList<T>(BoundList<T> nodes, Context context, Func<T, Context, T> lower) where T : BoundNode
    {
        ImmutableArray<T>.Builder? builder = null;
        for (var i = 0; i < nodes.Count; ++i)
        {
            var oldNode = nodes[i];
            var newNode = lower(oldNode, context);
            if (!ReferenceEquals(newNode, oldNode) && builder is null)
            {
                builder = ImmutableArray.CreateBuilder<T>(nodes.Count);
                for (var j = 0; j < i; ++j)
                    builder.Add(nodes[j]);
            }
            builder?.Add(newNode);
        }

        return builder?.ToImmutable() ?? default;
    }
}
