using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Statements;

namespace CodeAnalysis.Parsing;

internal static class ParserStatement
{
    extension(SyntaxTokenStream stream)
    {
        public StatementSyntax ParseStatement() => stream.Current.SyntaxKind switch
        {
            SyntaxKind.IfKeyword => stream.ParseIfElseStatement(),
            //SyntaxKind.ForKeyword => stream.ParseForStatement(),
            SyntaxKind.WhileKeyword => stream.ParseWhileStatement(),
            SyntaxKind.BreakKeyword => stream.ParseBreakStatement(),
            SyntaxKind.ContinueKeyword => stream.ParseContinueStatement(),
            SyntaxKind.ReturnKeyword => stream.ParseReturnStatement(),
            SyntaxKind.BraceOpenToken => stream.ParseBlockStatement(),
            SyntaxKind.SemicolonToken => stream.ParseEmptyStatement(),
            _ => stream.ParseExpressionStatement(),
        };

        public StatementSyntax ParseExpressionStatement()
        {
            var expression = stream.ParseExpression();
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
            return new ExpressionStatementSyntax(expression, semicolonToken);
        }

        public StatementSyntax ParseBlockOrExpressionStatement() => stream.Current.SyntaxKind is SyntaxKind.BraceOpenToken
            ? stream.ParseBlockStatement()
            : stream.ParseExpressionStatement();

        private EmptyStatementSyntax ParseEmptyStatement()
        {
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

            return new EmptyStatementSyntax(semicolonToken);
        }

        private IfElseStatementSyntax ParseIfElseStatement()
        {
            var ifKeyword = stream.Match(SyntaxKind.IfKeyword);
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var condition = stream.ParseExpression();
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var then = stream.ParseStatement();
            var elseClause = stream.ParseElseClauseExpression();

            return new IfElseStatementSyntax(
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

            var @else = stream.ParseStatement();
            return new ElseClauseSyntax(elseKeyword, @else);
        }

        private WhileStatementSyntax ParseWhileStatement()
        {
            var whileKeyword = stream.Match(SyntaxKind.WhileKeyword);
            var parenthesisOpenToken = stream.Match(SyntaxKind.ParenthesisOpenToken);
            var condition = stream.ParseExpression();
            var parenthesisCloseToken = stream.Match(SyntaxKind.ParenthesisCloseToken);
            var body = stream.ParseStatement();
            return new WhileStatementSyntax(
                whileKeyword,
                parenthesisOpenToken,
                condition,
                parenthesisCloseToken,
                body);
        }

        private BreakStatementSyntax ParseBreakStatement()
        {
            var breakKeyword = stream.Match(SyntaxKind.BreakKeyword);
            var expression = stream.Current.SyntaxKind is not SyntaxKind.SemicolonToken ? stream.ParseExpression() : null;
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
            return new BreakStatementSyntax(breakKeyword, expression, semicolonToken);
        }

        private ContinueStatementSyntax ParseContinueStatement()
        {
            var continueKeyword = stream.Match(SyntaxKind.ContinueKeyword);
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
            return new ContinueStatementSyntax(continueKeyword, semicolonToken);
        }

        private ReturnStatementSyntax ParseReturnStatement()
        {
            var returnKeyword = stream.Match(SyntaxKind.ReturnKeyword);
            var expression = stream.Current.SyntaxKind is not SyntaxKind.SemicolonToken ? stream.ParseExpression() : null;
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);
            return new ReturnStatementSyntax(returnKeyword, expression, semicolonToken);
        }
    }
}
