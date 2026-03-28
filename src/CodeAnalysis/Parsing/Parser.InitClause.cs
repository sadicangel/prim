using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static InitClauseSyntax? ParseInitClause(SyntaxIterator iterator, [DoesNotReturnIf(false)] bool isOptional)
    {
        if (isOptional)
        {
            if (iterator.TryMatch(out var equalsToken, SyntaxKind.EqualsToken))
            {
                var expression = ParseExpression(iterator);
                return new InitClauseSyntax(equalsToken, expression);
            }

            return null;
        }
        else
        {
            var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
            var expression = ParseExpression(iterator);

            return new InitClauseSyntax(equalsToken, expression);
        }
    }
}
