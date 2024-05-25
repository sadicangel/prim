using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator, bool isTerminated)
    {
        return iterator.Current.SyntaxKind switch
        {
            SyntaxKind.BraceOpenToken => ParseBlockExpression(syntaxTree, iterator),
            //SyntaxKind.IfKeyword => ParseIfElseExpression(syntaxTree, iterator),
            //SyntaxKind.ForKeyword => ParseForExpression(syntaxTree, iterator),
            //SyntaxKind.WhileKeyword => ParseWhileExpression(syntaxTree, iterator),
            //SyntaxKind.BreakKeyword => ParseBreakExpression(syntaxTree, iterator),
            //SyntaxKind.ContinueKeyword => ParseContinueExpression(syntaxTree, iterator),
            //SyntaxKind.ReturnKeyword => ParseReturnExpression(syntaxTree, iterator),
            SyntaxKind.SemicolonToken when isTerminated => ParseEmptyExpression(syntaxTree, iterator),
            _ when isTerminated => ParseStatementExpression(syntaxTree, iterator),
            _ => ParseBinaryExpression(syntaxTree, iterator),
        };
    }
}
