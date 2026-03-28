using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ElseClauseSyntax? ParseElseClauseExpression(SyntaxIterator iterator)
    {
        if (!iterator.TryMatch(out var elseKeyword, SyntaxKind.ElseKeyword))
        {
            return null;
        }

        var @else = ParseExpression(iterator);
        return new ElseClauseSyntax(elseKeyword, @else);
    }
}
