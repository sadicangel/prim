using System.ComponentModel;
using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        return ParseExpressionPrivate(syntaxTree, iterator, isTerminated: false);
    }

    private static ExpressionSyntax ParseTerminatedExpression(SyntaxTree syntaxTree, SyntaxTokenIterator iterator)
    {
        return ParseExpressionPrivate(syntaxTree, iterator, isTerminated: true);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    private static ExpressionSyntax ParseExpressionPrivate(SyntaxTree syntaxTree, SyntaxTokenIterator iterator, bool isTerminated)
    {
        return iterator.Current.SyntaxKind switch
        {
            //SyntaxKind.IfKeyword => ParseIfElseExpression(syntaxTree, iterator),
            //SyntaxKind.ForKeyword => ParseForExpression(syntaxTree, iterator),
            //SyntaxKind.WhileKeyword => ParseWhileExpression(syntaxTree, iterator),
            //SyntaxKind.BreakKeyword => ParseBreakExpression(syntaxTree, iterator),
            //SyntaxKind.ContinueKeyword => ParseContinueExpression(syntaxTree, iterator),
            //SyntaxKind.ReturnKeyword => ParseReturnExpression(syntaxTree, iterator),
            SyntaxKind.BraceOpenToken when iterator.Peek(1).SyntaxKind is SyntaxKind.DotToken => ParseStructExpression(syntaxTree, iterator),
            SyntaxKind.BraceOpenToken => ParseBlockExpression(syntaxTree, iterator),
            SyntaxKind.BracketOpenToken => ParseArrayExpression(syntaxTree, iterator),
            SyntaxKind.SemicolonToken => ParseEmptyExpression(syntaxTree, iterator),
            _ when isTerminated => ParseStatementExpression(syntaxTree, iterator),
            _ => ParseBinaryExpression(syntaxTree, iterator),
        };
    }
}
