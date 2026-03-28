using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static SeparatedSyntaxList<TNode> ParseSyntaxList<TNode>(
        SyntaxIterator iterator,
        SyntaxKind separatorKind,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseDelegate<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();

        var parseNext = true;
        while (parseNext && !endingKinds.Contains(iterator.Current.SyntaxKind))
        {
            var start = iterator.Current;

            var node = parseNode(iterator);
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
