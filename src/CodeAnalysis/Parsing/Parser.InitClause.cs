using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static InitClauseSyntax? ParseInitClause(SyntaxTokenStream stream, [DoesNotReturnIf(false)] bool isOptional)
    {
        if (isOptional)
        {
            if (stream.TryMatch(out var equalsToken, SyntaxKind.EqualsToken))
            {
                var expression = ParseExpression(stream);
                return new InitClauseSyntax(equalsToken, expression);
            }

            return null;
        }
        else
        {
            var equalsToken = stream.Match(SyntaxKind.EqualsToken);
            var expression = ParseExpression(stream);

            return new InitClauseSyntax(equalsToken, expression);
        }
    }
}
