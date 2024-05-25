using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ReadOnlyList<TNode> ParseSyntaxList<TNode>(
        SyntaxTree syntaxTree,
        SyntaxTokenIterator iterator,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseNode<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = new ReadOnlyList<TNode>();

        var parseNext = true;
        while (parseNext && !endingKinds.Contains(iterator.Current.SyntaxKind))
        {
            var node = parseNode(syntaxTree, iterator);
            nodes.Add(node);
        }

        return new(nodes);
    }
}

file static class SyntaxKindSpanExtensions
{
    public static bool Contains(this ReadOnlySpan<SyntaxKind> l, SyntaxKind v)
    {
        for (int i = 0; i < l.Length; ++i)
            if (l[i] == v)
                return true;
        return false;
    }
}
