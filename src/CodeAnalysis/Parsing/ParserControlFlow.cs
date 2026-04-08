using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.ControlFlow;

namespace CodeAnalysis.Parsing;

internal static class ParserControlFlow
{
    extension(SyntaxTokenStream stream)
    {
        public IfElseExpressionSyntax ParseIfElseExpression()
        {
            var ifKeyword = stream.Match(SyntaxKind.IfKeyword);
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var condition = stream.ParseExpression();
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var then = stream.ParseExpression();
            var elseClause = stream.ParseElseClauseExpression();

            return new IfElseExpressionSyntax(
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

            var @else = stream.ParseExpressionTerminated();
            return new ElseClauseSyntax(elseKeyword, @else);
        }

        public WhileExpressionSyntax ParseWhileExpression()
        {
            var whileKeyword = stream.Match(SyntaxKind.WhileKeyword);
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var condition = stream.ParseExpression();
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var body = stream.ParseExpression();
            return new WhileExpressionSyntax(
                whileKeyword,
                parenthesisOpenToken,
                condition,
                parenthesisCloseToken,
                body);
        }

        public BreakExpressionSyntax ParseBreakExpression()
        {
            var breakKeyword = stream.Match(SyntaxKind.BreakKeyword);
            if (stream.Current.SyntaxKind is SyntaxKind.SemicolonToken)
            {
                var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
                return new BreakExpressionSyntax(breakKeyword, semicolonToken);
            }

            var expression = stream.ParseExpressionTerminated();
            return new BreakExpressionSyntax(breakKeyword, expression);
        }

        public ContinueExpressionSyntax ParseContinueExpression()
        {
            var continueKeyword = stream.Match(SyntaxKind.ContinueKeyword);
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
            return new ContinueExpressionSyntax(continueKeyword, semicolonToken);
        }

        public ReturnExpressionSyntax ParseReturnExpression()
        {
            var returnKeyword = stream.Match(SyntaxKind.ReturnKeyword);
            if (stream.Current.SyntaxKind is SyntaxKind.SemicolonToken)
            {
                var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
                return new ReturnExpressionSyntax(returnKeyword, null, semicolonToken);
            }

            var expression = stream.ParseExpressionTerminated();
            return new ReturnExpressionSyntax(returnKeyword, expression, null);
        }
    }
}
