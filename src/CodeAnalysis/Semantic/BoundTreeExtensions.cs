namespace CodeAnalysis.Semantic;

internal static class BoundTreeExtensions
{
    extension(BoundNode node)
    {
        public BoundNode LinkParents(BoundNode? parent = null)
        {
            node.SetParent(parent);

            foreach (var child in node.Children().OfType<BoundNode>())
            {
                child.LinkParents(node);
            }

            return node;
        }

        public BoundNode GetRoot()
        {
            var current = node;
            while (current.Parent is not null)
            {
                current = current.Parent;
            }

            return current;
        }

        public IEnumerable<BoundNode> EnumerateAncestors()
        {
            var current = node.Parent;
            while (current is not null)
            {
                yield return current;
                current = current.Parent;
            }
        }
    }
}
