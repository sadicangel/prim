using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static TypeClauseSyntax? ParseTypeClause(SyntaxIterator iterator, [DoesNotReturnIf(false)] bool isOptional)
    {
        if (isOptional)
        {
            if (iterator.TryMatch(out var colonToken, SyntaxKind.ColonToken))
            {
                var type = ParseType(iterator);
                return new TypeClauseSyntax(colonToken, type);
            }

            return null;
        }
        else
        {
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(iterator);

            return new TypeClauseSyntax(colonToken, type);
        }
    }
}
