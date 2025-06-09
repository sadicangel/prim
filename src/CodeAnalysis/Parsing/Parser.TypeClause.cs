using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static TypeClauseSyntax? ParseTypeClause(SyntaxTree syntaxTree, SyntaxIterator iterator, [DoesNotReturnIf(false)] bool isOptional)
    {
        if (isOptional)
        {
            if (iterator.TryMatch(out var colonToken, SyntaxKind.ColonToken))
            {
                var type = ParseType(syntaxTree, iterator);
                return new TypeClauseSyntax(syntaxTree, colonToken, type);
            }

            return null;
        }
        else
        {
            var colonToken = iterator.Match(SyntaxKind.ColonToken);
            var type = ParseType(syntaxTree, iterator);

            return new TypeClauseSyntax(syntaxTree, colonToken, type);
        }
    }
}
