using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static TypeClauseSyntax? ParseTypeClause(SyntaxTokenStream stream, [DoesNotReturnIf(false)] bool isOptional)
    {
        if (isOptional)
        {
            if (stream.TryMatch(out var colonToken, SyntaxKind.ColonToken))
            {
                var type = ParseType(stream);
                return new TypeClauseSyntax(colonToken, type);
            }

            return null;
        }
        else
        {
            var colonToken = stream.Match(SyntaxKind.ColonToken);
            var type = ParseType(stream);

            return new TypeClauseSyntax(colonToken, type);
        }
    }
}
