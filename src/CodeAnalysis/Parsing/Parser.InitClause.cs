using System.Diagnostics.CodeAnalysis;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static InitClauseSyntax? ParseInitClause(SyntaxTree syntaxTree, SyntaxIterator iterator, [DoesNotReturnIf(false)] bool isOptional)
    {
        if (isOptional)
        {
            if (iterator.TryMatch(out var equalsToken, SyntaxKind.EqualsToken))
            {
                var expression = ParseExpression(syntaxTree, iterator);
                return new InitClauseSyntax(syntaxTree, equalsToken, expression);
            }

            return null;
        }
        else
        {

            var equalsToken = iterator.Match(SyntaxKind.EqualsToken);
            var expression = ParseExpression(syntaxTree, iterator);

            return new InitClauseSyntax(syntaxTree, equalsToken, expression);
        }
    }
}
