using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static SyntaxList<TNode> ParseSyntaxList<TNode>(
        SyntaxIterator iterator,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseDelegate<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = ImmutableArray.CreateBuilder<TNode>();

        while (!iterator.Current.SyntaxKind.IsEndingKind(endingKinds))
        {
            var start = iterator.Current;

            nodes.Add(parseNode(iterator));

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (iterator.Current == start)
                _ = iterator.Match();
        }

        return new SyntaxList<TNode>(nodes.ToImmutable());
    }
}
