using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static SeparatedSyntaxList<TNode> ParseSeparatedSyntaxList<TNode>(
        SyntaxTree syntaxTree,
        SyntaxIterator iterator,
        SyntaxKind separatorKind,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseNode<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();

        var parseNext = true;
        while (parseNext && !endingKinds.Contains(iterator.Current.SyntaxKind))
        {
            var start = iterator.Current;

            var node = parseNode(syntaxTree, iterator);
            nodes.Add(node);

            if (iterator.TryMatch(out var separatorToken, separatorKind))
                nodes.Add(separatorToken);
            else
                parseNext = false;

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (iterator.Current == start)
                _ = iterator.Match();
        }

        return new SeparatedSyntaxList<TNode>(nodes.ToImmutable());
    }
}

file static class SyntaxKindSpanExtensions
{
    public static bool Contains(this ReadOnlySpan<SyntaxKind> l, SyntaxKind v)
    {
        for (var i = 0; i < l.Length; ++i)
            if (l[i] == v)
                return true;
        return false;
    }
}
