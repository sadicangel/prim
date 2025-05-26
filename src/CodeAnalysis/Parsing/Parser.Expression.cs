using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;
partial class Parser
{
    private static ExpressionSyntax ParseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator) =>
        ParseExpression(syntaxTree, iterator, allowUnterminated: true);

    public static ExpressionSyntax ParseExpression(SyntaxTree syntaxTree, SyntaxIterator iterator, bool allowUnterminated) => iterator.Current.SyntaxKind switch
    {
        SyntaxKind.IfKeyword => Parser.ParseIfElseExpression(syntaxTree, iterator),
        ////SyntaxKind.ForKeyword => Parser.ParseForExpression(syntaxTree, iterator),
        //SyntaxKind.WhileKeyword => Parser.ParseWhileExpression(syntaxTree, iterator),
        //SyntaxKind.BreakKeyword => Parser.ParseBreakExpression(syntaxTree, iterator),
        //SyntaxKind.ContinueKeyword => Parser.ParseContinueExpression(syntaxTree, iterator),
        //SyntaxKind.ReturnKeyword => Parser.ParseReturnExpression(syntaxTree, iterator),
        SyntaxKind.BraceOpenToken => Parser.ParseBlockExpression(syntaxTree, iterator),
        SyntaxKind.BracketOpenToken => Parser.ParseArrayInitializerExpression(syntaxTree, iterator),
        SyntaxKind.SemicolonToken => Parser.ParseEmptyExpression(syntaxTree, iterator),
        _ => allowUnterminated ? Parser.ParseBinaryExpression(syntaxTree, iterator) : Parser.ParseStatementExpression(syntaxTree, iterator),
    };
}
