using CodeAnalysis.Syntax;
using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Parsing;

internal partial class Parser
{
    private static ExpressionSyntax ParseExpression(SyntaxIterator iterator) =>
        ParseExpression(iterator, allowUnterminated: true);

    public static ExpressionSyntax ParseExpression(SyntaxIterator iterator, bool allowUnterminated) => iterator.Current.SyntaxKind switch
    {
        SyntaxKind.ModuleKeyword => ParseModuleDeclaration(iterator),
        SyntaxKind.StructKeyword => ParseStructDeclaration(iterator),
        SyntaxKind.LetKeyword => ParseVariableDeclaration(iterator),
        SyntaxKind.VarKeyword => ParseVariableDeclaration(iterator),
        SyntaxKind.IfKeyword => ParseIfElseExpression(iterator),
        //SyntaxKind.ForKeyword => ParseForExpression(iterator),
        //SyntaxKind.WhileKeyword => ParseWhileExpression(iterator),
        //SyntaxKind.BreakKeyword => ParseBreakExpression(iterator),
        //SyntaxKind.ContinueKeyword => ParseContinueExpression(iterator),
        //SyntaxKind.ReturnKeyword => ParseReturnExpression(iterator),
        SyntaxKind.BraceOpenToken => ParseBlockExpression(iterator),
        SyntaxKind.BracketOpenToken => ParseArrayInitializerExpression(iterator),
        SyntaxKind.SemicolonToken => ParseEmptyExpression(iterator),
        _ => allowUnterminated ? ParseBinaryExpression(iterator) : ParseStatementExpression(iterator),
    };
}
