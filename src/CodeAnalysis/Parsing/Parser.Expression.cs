using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ExpressionSyntax ParseExpression(SyntaxTokenStream stream) =>
        ParseExpression(stream, allowUnterminated: true);

    public static ExpressionSyntax ParseExpression(SyntaxTokenStream stream, bool allowUnterminated) => stream.Current.SyntaxKind switch
    {
        SyntaxKind.IfKeyword => ParseIfElseExpression(stream),
        //SyntaxKind.ForKeyword => ParseForExpression(iterator),
        //SyntaxKind.WhileKeyword => ParseWhileExpression(iterator),
        //SyntaxKind.BreakKeyword => ParseBreakExpression(iterator),
        //SyntaxKind.ContinueKeyword => ParseContinueExpression(iterator),
        //SyntaxKind.ReturnKeyword => ParseReturnExpression(iterator),
        SyntaxKind.BraceOpenToken => ParseBlockExpression(stream),
        SyntaxKind.BracketOpenToken => ParseArrayInitializerExpression(stream),
        SyntaxKind.SemicolonToken => ParseEmptyExpression(stream),
        _ => allowUnterminated ? ParseBinaryExpression(stream) : ParseStatementExpression(stream),
    };
}
