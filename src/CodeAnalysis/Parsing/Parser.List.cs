using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static SyntaxList<TNode> ParseSyntaxList<TNode>(
        SyntaxTokenStream stream,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseDelegate<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = ImmutableArray.CreateBuilder<TNode>();

        while (!stream.Current.SyntaxKind.IsEndingKind(endingKinds))
        {
            var start = stream.Current;

            nodes.Add(parseNode(stream));

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (stream.Current == start)
                _ = stream.Match();
        }

        return new SyntaxList<TNode>(nodes.ToImmutable());
    }
}
