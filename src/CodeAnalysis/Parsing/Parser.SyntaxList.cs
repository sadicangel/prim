using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static SyntaxList<TNode> ParseSyntaxList<TNode>(
        SyntaxTree syntaxTree,
        SyntaxTokenIterator iterator,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseNode<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = new SyntaxList<TNode>.Builder();

        while (!endingKinds.Contains(iterator.Current.SyntaxKind))
        {
            var start = iterator.Current;

            nodes.Add(parseNode(syntaxTree, iterator));

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (iterator.Current == start)
                _ = iterator.Match();
        }

        return nodes.ToSyntaxList();
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
