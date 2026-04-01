using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ElseClauseSyntax? ParseElseClauseExpression(SyntaxTokenStream stream)
    {
        if (!stream.TryMatch(out var elseKeyword, SyntaxKind.ElseKeyword))
        {
            return null;
        }

        var @else = ParseExpression(stream);
        return new ElseClauseSyntax(elseKeyword, @else);
    }
}
