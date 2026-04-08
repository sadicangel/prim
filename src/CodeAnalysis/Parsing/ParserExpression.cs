using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal static class ParserExpression
{
    extension(SyntaxTokenStream stream)
    {
        public ExpressionSyntax ParseExpression() => stream.ParseExpression(allowUnterminated: true);
        public ExpressionSyntax ParseExpressionTerminated() => stream.ParseExpression(allowUnterminated: false);

        private ExpressionSyntax ParseExpression(bool allowUnterminated) => stream.Current.SyntaxKind switch
        {
            SyntaxKind.IfKeyword => stream.ParseIfElseExpression(),
            //SyntaxKind.ForKeyword => stream.ParseForExpression(),
            SyntaxKind.WhileKeyword => stream.ParseWhileExpression(),
            //SyntaxKind.BreakKeyword => stream.ParseBreakExpression(),
            //SyntaxKind.ContinueKeyword => stream.ParseContinueExpression(),
            //SyntaxKind.ReturnKeyword => stream.ParseReturnExpression(),
            SyntaxKind.BraceOpenToken => stream.ParseBlockExpression(),
            SyntaxKind.SemicolonToken => stream.ParseEmptyExpression(),
            _ => stream.ParseExpressionCore(allowUnterminated),
        };

        private ExpressionSyntax ParseExpressionCore(bool allowUnterminated)
        {
            var expression = stream.ParseBinaryExpression();
            if (allowUnterminated || expression.IsTerminated)
            {
                return expression;
            }

            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

            return new StatementExpressionSyntax(expression, semicolonToken);
        }

        private EmptyExpressionSyntax ParseEmptyExpression()
        {
            var semicolonToken = stream.Match(SyntaxKind.SemicolonToken);

            return new EmptyExpressionSyntax(semicolonToken);
        }
    }
}
