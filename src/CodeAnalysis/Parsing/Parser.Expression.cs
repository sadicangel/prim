using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator) =>
        ParseExpression(syntaxTree, iterator, allowUnterminated: true);

    public static ExpressionSyntax ParseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator, bool allowUnterminated) => iterator.Current.SyntaxKind switch
    {
        SyntaxKind.IfKeyword => ParseIfElseExpression(syntaxTree, iterator),
        //SyntaxKind.ForKeyword => ParseForExpression(syntaxTree, iterator),
        //SyntaxKind.WhileKeyword => ParseWhileExpression(syntaxTree, iterator),
        //SyntaxKind.BreakKeyword => ParseBreakExpression(syntaxTree, iterator),
        //SyntaxKind.ContinueKeyword => ParseContinueExpression(syntaxTree, iterator),
        //SyntaxKind.ReturnKeyword => ParseReturnExpression(syntaxTree, iterator),
        SyntaxKind.BraceOpenToken => ParseBlockExpression(syntaxTree, iterator),
        SyntaxKind.BracketOpenToken => ParseArrayInitializerExpression(syntaxTree, iterator),
        SyntaxKind.SemicolonToken => ParseEmptyExpression(syntaxTree, iterator),
        _ => allowUnterminated ? ParseBinaryExpression(syntaxTree, iterator) : ParseStatementExpression(syntaxTree, iterator),
    };
}
