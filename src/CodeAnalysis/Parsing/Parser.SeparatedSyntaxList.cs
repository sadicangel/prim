using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static List<SyntaxNode> ParseSeparatedSyntaxList<TNode>(
        SyntaxTree syntaxTree,
        SyntaxTokenIterator iterator,
        SyntaxKind separatorKind,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseNode<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = new List<SyntaxNode>();

        var parseNext = true;
        while (parseNext && !endingKinds.Contains(iterator.Current.SyntaxKind))
        {
            var node = parseNode(syntaxTree, iterator);
            nodes.Add(node);

            if (iterator.TryMatch(separatorKind, out var separatorToken))
                nodes.Add(separatorToken);
            else
                parseNext = false;
        }

        return nodes;
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
