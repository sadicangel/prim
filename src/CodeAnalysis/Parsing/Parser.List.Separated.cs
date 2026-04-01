using System.Collections.Immutable;
using CodeAnalysis.Syntax;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static SeparatedSyntaxList<TNode> ParseSyntaxList<TNode>(
        SyntaxTokenStream stream,
        SyntaxKind separatorKind,
        ReadOnlySpan<SyntaxKind> endingKinds,
        ParseDelegate<TNode> parseNode)
        where TNode : SyntaxNode
    {
        var nodes = ImmutableArray.CreateBuilder<SyntaxNode>();

        var parseNext = true;
        while (parseNext && !endingKinds.Contains(stream.Current.SyntaxKind))
        {
            var start = stream.Current;

            var node = parseNode(stream);
            nodes.Add(node);

            if (stream.TryMatch(out var separatorToken, separatorKind))
                nodes.Add(separatorToken);
            else
                parseNext = false;

            // No tokens consumed. Skip the current token to avoid infinite loop.
            // No need to report any extra error as parse methods already failed.
            if (stream.Current == start)
                _ = stream.Match();
        }

        return new SeparatedSyntaxList<TNode>(nodes.ToImmutable());
    }
}
