using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;

internal static class ParserControlFlow
{
    extension(SyntaxTokenStream stream)
    {
        public IfExpressionSyntax ParseIfElseExpression()
        {
            var ifKeyword = stream.Match(SyntaxKind.IfKeyword);
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var condition = stream.ParseExpression();
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var then = stream.ParseExpression();
            var elseClause = stream.ParseElseClauseExpression();

            return new IfExpressionSyntax(
                ifKeyword,
                parenthesisOpenToken,
                condition,
                parenthesisCloseToken,
                then,
                elseClause);
        }

        private ElseClauseSyntax? ParseElseClauseExpression()
        {
            if (!stream.TryMatch(out var elseKeyword, SyntaxKind.ElseKeyword))
            {
                return null;
            }

            var @else = stream.ParseExpression();
            return new ElseClauseSyntax(elseKeyword, @else);
        }
    }
}
