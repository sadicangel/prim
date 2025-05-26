using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions.ControlFlow;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ElseClauseExpressionSyntax? ParseElseClauseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator)
    {
        if (!iterator.TryMatch(out var elseKeyword, SyntaxKind.ElseKeyword))
        {
            return null;
        }

        var @else = ParseExpression(syntaxTree, iterator);
        return new ElseClauseExpressionSyntax(syntaxTree, elseKeyword, @else);
    }
}
