using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ElseClauseSyntax? ParseElseClauseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        if (!iterator.TryMatch(out var elseKeyword, SyntaxKind.ElseKeyword))
        {
            return null;
        }

        var @else = ParseExpression(syntaxTree, iterator);
        return new ElseClauseSyntax(syntaxTree, elseKeyword, @else);
    }
}
